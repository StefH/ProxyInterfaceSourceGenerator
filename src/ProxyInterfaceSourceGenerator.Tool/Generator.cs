using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.Tool;

internal class Generator : IDisposable
{
    private readonly string _sourceDll;
    private readonly string _sourceFile;
    private readonly string _outputPath;

    private readonly ChannelWriter<FileData> _writer;
    private readonly ChannelReader<FileData> _reader;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Generator(IConfiguration configuration)
    {
        _sourceDll = configuration["sourceDll"] ?? throw new ArgumentNullException();
        _sourceFile = configuration["sourceFile"] ?? throw new ArgumentNullException();
        _outputPath = configuration["outputPath"] ?? ".";

        // Create unbounded channel for file processing queue
        var fileDataQueue = Channel.CreateUnbounded<FileData>();
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

        var allText = await File.ReadAllTextAsync(_sourceFile, cancellationToken);

        var syntaxTree = CSharpSyntaxTree.ParseText(allText);

        var compilation = CSharpCompilation.Create(
            "GeneratedNamespace_" + Guid.NewGuid().ToString("N"),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Create combined cancellation token
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

        // Start the file processing task
        //var fileProcessingTask = ProcessFileQueueAsync(references, combinedCts.Token);

        var simplifier = new CSharpSimplifier(references);

        try
        {
            // Run the generator
            _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator(fileData => GenerateFileAction(fileData, simplifier)))
                .RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

            // Signal that no more files will be enqueued
            //_writer.Complete();

            // Wait for all files to be processed
            //await fileProcessingTask;
        }
        catch
        {
            //await _cancellationTokenSource.CancelAsync();
            throw;
        }
    }

    private void GenerateFileAction(FileData fileData, CSharpSimplifier simplifier)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var fullPath = Path.Combine(_outputPath, fileData.Filename);
        // Console.WriteLine($"Processing file: {fileData.Filename}");

        //string modified;
        //try
        //{
        //    modified = simplifier.SimplifyCSharpCodeAsync(fileData.Text).GetAwaiter().GetResult();
        //}
        //catch (Exception ex)
        //{
        //    modified = fileData.Text; // Fall back to original content

        //    Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
        //}

        File.WriteAllText(fullPath, fileData.Text);

        stopwatch.Stop();
        Console.WriteLine($"Written file: {fileData.Filename} ({stopwatch.Elapsed.TotalMilliseconds} ms)");

        //if (!_writer.TryWrite(fileData))
        //{
        //    Console.WriteLine($"Warning: Failed to enqueue file {fileData.Filename}");
        //}
    }

    private async Task ProcessFileQueueAsync(HashSet<MetadataReference> references, CancellationToken cancellationToken)
    {
        var simplifier = new CSharpSimplifier(references);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 1, // Environment.ProcessorCount,
            CancellationToken = cancellationToken
        };

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        int idx = 0;

        await Parallel.ForEachAsync(
            _reader.ReadAllAsync(cancellationToken),
            parallelOptions,
            async (fileData, ct) =>
            {
                var fullPath = Path.Combine(_outputPath, fileData.Filename);
                // Console.WriteLine($"Processing file: {fileData.Filename}");

                string modified;
                try
                {
                    modified = await simplifier.SimplifyCSharpCodeAsync(fileData.Text);
                }
                catch (Exception ex)
                {
                    modified = fileData.Text; // Fall back to original content

                    Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
                }

                await File.WriteAllTextAsync(fullPath, modified, ct);

                
                Console.WriteLine($"Written file: {fileData.Filename}");

                idx++;

                if (idx > 100)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds} s");
                    throw new AccessViolationException();
                }

                //  1 = 13,7243953 s
                //  2 = 12,0959904 s
                //  4 = 11,9327242 s
                //  8 = 12,4146649 s
                // 32 = 15,0603359 s
            });
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}