using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public class CSharpSimplifier : IDisposable
{
    private readonly AdhocWorkspace _workspace;
    private readonly Project _project;

    public CSharpSimplifier(HashSet<MetadataReference> references)
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

        var newDoc = document.WithSyntaxRoot(annotatedRoot);

        var simplifiedDoc = await Simplifier.ReduceAsync(newDoc, cancellationToken: cancellationToken);

        return (await simplifiedDoc.GetTextAsync(cancellationToken)).ToString();
    }
}