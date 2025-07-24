using System.Threading.Channels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.Tool;

internal class Generator
{
    private readonly string _sourceDll;
    private readonly string _sourceFile;
    private readonly string _outputPath;

    private CSharpSimplifier _simplifier;
    private readonly Channel<FileData> _fileQueue;
    private readonly ChannelWriter<FileData> _writer;
    private readonly ChannelReader<FileData> _reader;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Generator(IConfiguration configuration)
    {
        _sourceDll = configuration["sourceDll"] ?? throw new ArgumentNullException();
        _sourceFile = configuration["sourceFile"] ?? throw new ArgumentNullException();
        _outputPath = configuration["outputPath"] ?? ".";

        // Create unbounded channel for file processing queue
        _fileQueue = Channel.CreateUnbounded<FileData>();
        _writer = _fileQueue.Writer;
        _reader = _fileQueue.Reader;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var references = MetadataReferenceUtils.GetAllReferences(_sourceDll);

        _simplifier = new CSharpSimplifier(references);

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
        var fileProcessingTask = ProcessFileQueueAsync(combinedCts.Token);

        try
        {
            // Run the generator
            _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator(GenerateFileAction))
                .RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

            // Signal that no more files will be enqueued
            _writer.Complete();

            // Wait for all files to be processed
            await fileProcessingTask;
        }
        catch
        {
            _cancellationTokenSource.Cancel();
            throw;
        }
    }

    private void GenerateFileAction(FileData fileData)
    {
        if (!_writer.TryWrite(fileData))
        {
            Console.WriteLine($"Warning: Failed to enqueue file {fileData.Filename}");
        }
    }

    private async Task ProcessFileQueueAsync(CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(Environment.ProcessorCount); // Limit concurrent file operations
        var tasks = new List<Task>();

        try
        {
            await foreach (var fileData in _reader.ReadAllAsync(cancellationToken))
            {
                await semaphore.WaitAsync(cancellationToken);

                var processTask = ProcessSingleFileAsync(fileData, semaphore, cancellationToken);
                tasks.Add(processTask);
            }

            // Wait for all file processing tasks to complete
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    private async Task ProcessSingleFileAsync(FileData fileData, SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        try
        {
            var fullPath = Path.Combine(_outputPath, fileData.Filename);
            Console.WriteLine($"Processing file: {fullPath}");

            string modified;
            try
            {
                modified = await _simplifier.SimplifyCSharpCodeAsync(fileData.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simplifying code for {fileData.Filename}: {ex.Message}");
                modified = fileData.Text; // Fall back to original content
            }

            await File.WriteAllTextAsync(fullPath, modified, cancellationToken);
            Console.WriteLine($"Written file: {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {fileData.Filename}: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}