using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public class CSharpSimplifier1 : IDisposable
{
    private readonly AdhocWorkspace _workspace;
    private readonly Project _project;

    public CSharpSimplifier1(HashSet<MetadataReference> references)
    {
        var id = Guid.NewGuid().ToString("N");

        _workspace = new AdhocWorkspace();

        _project = _workspace
            .CurrentSolution
            .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
            .WithMetadataReferences(references);
    }

    public void Dispose()
    {
        _workspace.Dispose();
    }

    public async Task<string> SimplifyCSharpCodeAsync(string sourceCode, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString("N");

        var document = _project.AddDocument($"SourceFile_{id}.cs", SourceText.From(sourceCode));

        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var annotatedRoot = root!.WithAdditionalAnnotations(Simplifier.Annotation);

        var newDoc = document.WithSyntaxRoot(root!);

        var simplifiedDoc = await Simplifier.ReduceAsync(document, cancellationToken: cancellationToken);

        return (await simplifiedDoc.GetTextAsync(cancellationToken)).ToString();
    }
}

public class CSharpSimplifier : IDisposable
{
    private readonly AdhocWorkspace _workspace;

    public CSharpSimplifier()
    {
        _workspace = new AdhocWorkspace();
    }

    public void Dispose()
    {
        _workspace.Dispose();
    }

    /// <summary>
    /// Simplifies a collection of C# source codes using controlled parallelism to avoid resource saturation.
    /// </summary>
    /// <param name="sourceCodes">A dictionary mapping a unique key (like a hint name) to the source code.</param>
    /// <param name="references">The metadata references required for compilation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping the original key to the simplified source code.</returns>
    public async Task<Dictionary<string, string>> SimplifyCSharpCodeWithControlledParallelismAsync(
        IEnumerable<(string Filename, byte[] Data)> sourceCodes,
        IEnumerable<MetadataReference> references,
        CancellationToken cancellationToken)
    {
        // 1. Determine the optimal degree of parallelism.
        // A good starting point is the number of processors, but for memory-intensive tasks,
        // half the processors can sometimes be better. This is a value to tune.
        int maxDegreeOfParallelism = Environment.ProcessorCount;

        // 2. Setup the single project with all documents (this part is efficient).
        var projectId = ProjectId.CreateNewId();
        Solution solution = _workspace.CurrentSolution
            .AddProject(projectId, "SimplificationProject", "SimplificationProject.dll", LanguageNames.CSharp);
        solution = solution.WithProjectMetadataReferences(projectId, references);

        var documentInfo = new List<(string Key, DocumentId Id)>();
        foreach (var entry in sourceCodes)
        {
            var text = await StringCompressor.DecompressAsync(entry.Data);
            var sourceText = SourceText.From(text);
            var docId = DocumentId.CreateNewId(projectId, debugName: entry.Filename);
            solution = solution.AddDocument(docId, entry.Filename, sourceText);
            documentInfo.Add((entry.Filename, docId));
        }

        // 3. Use SemaphoreSlim to control concurrency.
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);

        // 4. Create a list of tasks, each responsible for simplifying one document.
        var processingTasks = new List<Task<KeyValuePair<string, string>>>();

        foreach (var (key, docId) in documentInfo)
        {
            // This task will wait until the semaphore has a free slot.
            var task = ProcessSingleDocumentAsync(solution.GetDocument(docId)!, key, semaphore, cancellationToken);
            processingTasks.Add(task);
        }

        // 5. Await all tasks to complete and collect the results.
        var results = await Task.WhenAll(processingTasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    // This helper now includes the semaphore logic.
    private async Task<KeyValuePair<string, string>> ProcessSingleDocumentAsync(
        Document document,
        string key,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        // Wait until a slot is free in the semaphore.
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            var simplifiedText = await SimplifyDocumentAsync(document, cancellationToken);
            return new KeyValuePair<string, string>(key, simplifiedText);
        }
        finally
        {
            // CRITICAL: Always release the semaphore slot, even if an exception occurs.
            semaphore.Release();
        }
    }

    private static async Task<string> SimplifyDocumentAsync(Document document, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root is null)
        {
            var text = await document.GetTextAsync(cancellationToken);
            return text.ToString();
        }

        var annotatedDocument = document.WithSyntaxRoot(root.WithAdditionalAnnotations(Simplifier.Annotation));
        var simplifiedDoc = await Simplifier.ReduceAsync(annotatedDocument, cancellationToken: cancellationToken);
        var simplifiedText = await simplifiedDoc.GetTextAsync(cancellationToken);
        return simplifiedText.ToString();
    }
}