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

        // var codeBlocks = await GetCodeBlocksByNamespacePrefixAsync(_sourceFile);

        var allText = File.ReadAllText(_sourceFile);

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
        var fileProcessingTask = ProcessFileQueueAsync(references, combinedCts.Token);

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Run the generator
            _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator(fileData =>
                {
                    // Use compression to reduce memory usage during file processing
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
        }
        catch
        {
            await _cancellationTokenSource.CancelAsync();
            throw;
        }
    }

    private async Task<string> GetCodeBlocksByNamespacePrefixAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(_sourceNamespace) || _sourceNamespace == "*")
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

    private async Task ProcessFileQueueAsync(HashSet<MetadataReference> references, CancellationToken cancellationToken)
    {
        var idx = 0;

        await foreach (var fileData in _reader.ReadAllAsync(cancellationToken))
        {
            var fullPath = Path.Combine(_outputPath, fileData.Filename);
            // Console.WriteLine($"Processing file: {fileData.Filename}");

            var text = await StringCompressor.DecompressAsync(fileData.Data);
            string modified;
            try
            {
                using var simplifier = new CSharpSimplifier(references);
                modified = await simplifier.SimplifyCSharpCodeAsync(text, cancellationToken);
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
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}