using System.Threading.Channels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;

namespace ProxyInterfaceSourceGenerator.Tool;

internal class Generator
{
    private readonly string _sourceDll;
    private readonly string _sourceFile;
    private readonly string _outputPath;

    private CSharpSimplifier _simplifier;
    private readonly Channel<FileTask> _fileQueue;
    private readonly ChannelWriter<FileTask> _writer;
    private readonly ChannelReader<FileTask> _reader;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Generator(IConfiguration configuration)
    {
        _sourceDll = configuration["sourceDll"] ?? throw new ArgumentNullException();
        _sourceFile = configuration["sourceFile"] ?? throw new ArgumentNullException();
        _outputPath = configuration["outputPath"] ?? ".";

        // Create unbounded channel for file processing queue
        _fileQueue = Channel.CreateUnbounded<FileTask>();
        _writer = _fileQueue.Writer;
        _reader = _fileQueue.Reader;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task GenerateAsync()
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var references = MetadataReferenceUtils.GetAllReferences(_sourceDll);

        _simplifier = new CSharpSimplifier(references);

        var allText = File.ReadAllText(_sourceFile);

        var syntaxTree = CSharpSyntaxTree.ParseText(allText);

        var compilation = CSharpCompilation.Create(
            "GeneratedNamespace_" + Guid.NewGuid().ToString("N"),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Start the file processing task
        var fileProcessingTask = ProcessFileQueueAsync(_cancellationTokenSource.Token);

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

    private void GenerateFileAction(string fileName, string content)
    {
        // Enqueue the file for processing
        var fileTask = new FileTask(fileName, content);

        if (!_writer.TryWrite(fileTask))
        {
            Console.WriteLine($"Warning: Failed to enqueue file {fileName}");
        }
    }

    private async Task ProcessFileQueueAsync(CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(Environment.ProcessorCount); // Limit concurrent file operations
        var tasks = new List<Task>();

        try
        {
            await foreach (var fileTask in _reader.ReadAllAsync(cancellationToken))
            {
                await semaphore.WaitAsync(cancellationToken);

                var processTask = ProcessSingleFileAsync(fileTask, semaphore, cancellationToken);
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

    private async Task ProcessSingleFileAsync(FileTask fileTask, SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        try
        {
            var fullPath = Path.Combine(_outputPath, fileTask.FileName);
            Console.WriteLine($"Processing file: {fullPath}");

            string modified;
            try
            {
                modified = await _simplifier.SimplifyCSharpCodeAsync(fileTask.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simplifying code for {fileTask.FileName}: {ex.Message}");
                modified = fileTask.Content; // Fall back to original content
            }

            await File.WriteAllTextAsync(fullPath, modified, cancellationToken);
            Console.WriteLine($"Written file: {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {fileTask.FileName}: {ex.Message}");
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

    private record FileTask(string FileName, string Content);
}