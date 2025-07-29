using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;

namespace ProxyInterfaceSourceGenerator.Tool;

internal class Generator : IDisposable
{
    private readonly string _sourceDll;
    private readonly string _sourceFile;
    private readonly string? _sourceNamespace;
    private readonly string _outputPath;

    private readonly ChannelWriter<(string Filename, byte[] Data)> _writer;
    private readonly ChannelReader<(string Filename, byte[] Data)> _reader;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Generator(IConfiguration configuration)
    {
        _sourceDll = configuration["sourceDll"] ?? throw new ArgumentNullException();
        _sourceFile = configuration["sourceFile"] ?? throw new ArgumentNullException();
        _sourceNamespace = configuration["sourceNamespace"];
        _outputPath = configuration["outputPath"] ?? ".";

        if (!string.IsNullOrWhiteSpace(_sourceNamespace))
        {
            _outputPath = Path.Combine(_outputPath, _sourceNamespace.Split('.').Last());
        }

        // Create unbounded channel for file processing queue
        var fileDataQueue = Channel.CreateUnbounded<(string Filename, byte[] Data)>();
        _writer = fileDataQueue.Writer;
        _reader = fileDataQueue.Reader;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var references = MetadataReferenceUtils.GetAllReferences(_sourceDll);

        var codeBlocks = await GetCodeBlocksByNamespacePrefixAsync(_sourceFile);

        var syntaxTree = CSharpSyntaxTree.ParseText(codeBlocks);

        var compilation = CSharpCompilation.Create(
            "GeneratedNamespace_" + Guid.NewGuid().ToString("N"),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Create combined cancellation token
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

        // Start the file processing task
        var fileProcessingTask = ProcessFileQueueAsync(references, combinedCts.Token);

        //var simplifier = new CSharpSimplifier(references);

        //var q = new Queue<(string Filename, byte[] Data)>();

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Run the generator
            _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator(fileData =>
                {
                    //_writer.TryWrite(fileData);
                    //GenerateFileAction(fileData, simplifier);
                    //q.Enqueue((fileData.Filename, StringCompressor.Compress(fileData.Text)));
                    _writer.TryWrite((fileData.Filename, StringCompressor.Compress(fileData.Text)));

                }))
                .RunGenerators(compilation, cancellationToken);

            stopwatch.Stop();
            Console.WriteLine($"RunGenerators ({stopwatch.Elapsed.TotalSeconds} s)");

            // Signal that no more files will be enqueued
            _writer.Complete();

            stopwatch.Restart();

            // Wait for all files to be processed
            await fileProcessingTask;
            stopwatch.Stop();
            Console.WriteLine($"fileProcessingTask ({stopwatch.Elapsed.TotalSeconds} s)");

            // lege folder = fileProcessingTask (0,8354483 s)
            // 500 bestanden

            //foreach (var fileData in q)
            //{
            //    var stopwatch = new Stopwatch();
            //    stopwatch.Start();

            //    var fullPath = Path.Combine(_outputPath, fileData.Filename);
            //    Console.WriteLine($"Processing file: {fileData.Filename}");

            //    string text = await StringCompressor.DecompressAsync(fileData.Data);
            //    string modified;
            //    try
            //    {
            //        modified = await simplifier.SimplifyCSharpCodeAsync(text);
            //    }
            //    catch (Exception ex)
            //    {
            //        modified = text; // Fall back to original content

            //        Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
            //    }

            //    await File.WriteAllTextAsync(fullPath, modified, cancellationToken);

            //    stopwatch.Stop();
            //    Console.WriteLine($"Written file: {fileData.Filename} ({stopwatch.Elapsed.TotalMilliseconds} ms)");
            //}

        }
        catch
        {
            await _cancellationTokenSource.CancelAsync();
            throw;
        }
    }

    private async Task<string> GetCodeBlocksByNamespacePrefixAsync(string filePath)
    {
        if (_sourceNamespace == "*")
        {
            return await File.ReadAllTextAsync(filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        var result = new StringBuilder();
        var currentBlock = new StringBuilder();
        bool insideBlock = false;

        foreach (var line in lines)
        {
            // Detect start of a namespace
            if (line.TrimStart().StartsWith("namespace "))
            {
                // If we're inside a matching block, add it to result before starting a new one
                if (insideBlock && currentBlock.Length > 0)
                {
                    result.AppendLine(currentBlock.ToString().Trim());
                    result.AppendLine(); // Add spacing between blocks
                    currentBlock.Clear();
                }

                // Start a new block if namespace matches
                if (line.Contains(_sourceNamespace!))
                {
                    insideBlock = true;
                    currentBlock.AppendLine(line);
                }
                else
                {
                    insideBlock = false;
                }
            }
            else if (insideBlock)
            {
                currentBlock.AppendLine(line);
            }
        }

        // Add the last block if needed
        if (insideBlock && currentBlock.Length > 0)
        {
            result.AppendLine(currentBlock.ToString().Trim());
        }

        return result.ToString().Trim();
    }

    //private void GenerateFileAction(FileData fileData, CSharpSimplifier simplifier)
    //{
    //    var stopwatch = new Stopwatch();
    //    stopwatch.Start();

    //    var fullPath = Path.Combine(_outputPath, fileData.Filename);
    //    Console.WriteLine($"Processing file: {fileData.Filename}");

    //    string modified;
    //    try
    //    {
    //        modified = simplifier.SimplifyCSharpCodeAsync(fileData.Text).GetAwaiter().GetResult();
    //    }
    //    catch (Exception ex)
    //    {
    //        modified = fileData.Text; // Fall back to original content

    //        Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
    //    }

    //    File.WriteAllText(fullPath, modified);

    //    stopwatch.Stop();
    //    Console.WriteLine($"Written file: {fileData.Filename} ({stopwatch.Elapsed.TotalMilliseconds} ms)");

    //    //if (!_writer.TryWrite(fileData))
    //    //{
    //    //    Console.WriteLine($"Warning: Failed to enqueue file {fileData.Filename}");
    //    //}
    //}

    private async Task ProcessFileQueueAsync(HashSet<MetadataReference> references, CancellationToken cancellationToken)
    {
        var idx = 0;
        //const int batchSize = 200;
        //var batch = new List<(string Filename, string Text)>(batchSize);

        //await foreach (var fileData in _reader.ReadAllAsync(cancellationToken))
        //{
        //    // Decompress the data first
        //    var text = await StringCompressor.DecompressAsync(fileData.Data);
        //    batch.Add((fileData.Filename, text));

        //    //var currentIdx = Interlocked.Increment(ref idx);
        //    //Console.WriteLine($"DecompressAsync {currentIdx}");

        //    if (batch.Count >= batchSize)
        //    {
        //        await ProcessBatchAsync(references, batch, cancellationToken);
        //        batch.Clear();
        //    }
        //}

        //// Process any remaining items in the final batch
        //if (batch.Count > 0)
        //{
        //    await ProcessBatchAsync(references, batch, cancellationToken);
        //}
                //return;

        await foreach (var fileData in _reader.ReadAllAsync(cancellationToken))
        {
            var fullPath = Path.Combine(_outputPath, fileData.Filename);
            // Console.WriteLine($"Processing file: {fileData.Filename}");

            var text = await StringCompressor.DecompressAsync(fileData.Data);
            string modified;
            try
            {
                var simplifier = new CSharpSimplifier(references);
                modified = await simplifier.SimplifyCSharpCodeAsync(text);
            }
            catch (Exception ex)
            {
                modified = text; // Fall back to original content

                Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
            }

            await File.WriteAllTextAsync(fullPath, modified, cancellationToken);

            var currentIdx = Interlocked.Increment(ref idx);
            Console.WriteLine($"Written file: {fileData.Filename} {currentIdx}");
        }
    }

    //private async Task ProcessBatchAsync(HashSet<MetadataReference> references, List<(string Filename, string Text)> batchItems, CancellationToken cancellationToken)
    //{
    //    var simplifier = new CSharpSimplifier(references);

    //    var stopwatch = new Stopwatch();
    //    stopwatch.Start();

    //    int idx = 0;

    //    var parallelOptions = new ParallelOptions
    //    {
    //        MaxDegreeOfParallelism = Environment.ProcessorCount * 4,
    //        CancellationToken = cancellationToken
    //    };

    //    try
    //    {
    //        // Extract source codes for batch processing
    //        var sourceCodes = batchItems.Select(item => item.Text);

    //        // Get simplified results from batch API
    //        var simplifiedResults = new List<string>();
    //        await foreach (var result in simplifier.SimplifyCSharpCodesAsync(sourceCodes, cancellationToken))
    //        {
    //            simplifiedResults.Add(result);
    //        }

    //        // Write results in parallel
    //        await Parallel.ForAsync(0, batchItems.Count, parallelOptions, async (i, ct) =>
    //        {
    //            var item = batchItems[i];
    //            var modified = i < simplifiedResults.Count ? simplifiedResults[i] : item.Text; // Fallback to original
    //            var fullPath = Path.Combine(_outputPath, item.Filename);

    //            await File.WriteAllTextAsync(fullPath, modified, ct);

    //            var currentIdx = Interlocked.Increment(ref idx);
    //            Console.WriteLine($"Written file: {item.Filename} {currentIdx}");
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error processing batch: {ex.Message}");

    //        // Fallback: write original content for all files in batch
    //        await Parallel.ForEachAsync(batchItems, parallelOptions, async (item, ct) =>
    //        {
    //            var fullPath = Path.Combine(_outputPath, item.Filename);
    //            await File.WriteAllTextAsync(fullPath, item.Text, ct);

    //            var currentIdx = Interlocked.Increment(ref idx);
    //            Console.WriteLine($"Written file (fallback): {item.Filename} {currentIdx}");
    //        });
    //    }

    //    stopwatch.Stop();
    //    Console.WriteLine($"ProcessBatchAsync ({stopwatch.Elapsed.TotalSeconds} s)");
    //}

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}