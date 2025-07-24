using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public class CSharpSimplifier
{
    private readonly Project _project;

    public CSharpSimplifier(HashSet<MetadataReference> references)
    {
        var id = Guid.NewGuid().ToString("N");

        using var workspace = new AdhocWorkspace();

        _project = workspace
            .CurrentSolution
            .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
            .WithMetadataReferences(references);
    }

    //public async Task<string> SimplifyCSharpCodeAsync(string sourceCode)
    //{
    //    var id = Guid.NewGuid().ToString("N");

    //    using var workspace = new AdhocWorkspace();

    //    var project = workspace
    //        .CurrentSolution
    //        .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
    //        .WithMetadataReferences(references);

    //    var document = project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

    //    var root = await document.GetSyntaxRootAsync();

    //    var annotatedRoot = root!.WithAdditionalAnnotations(Simplifier.Annotation);

    //    var newDoc = document.WithSyntaxRoot(annotatedRoot);

    //    var simplifiedDoc = await Simplifier.ReduceAsync(newDoc, workspace.Options);

    //    return (await simplifiedDoc.GetTextAsync()).ToString();
    //}

    public async IAsyncEnumerable<string> SimplifyCSharpCodesAsync(IEnumerable<string> sourceCodes, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var sourceCode in sourceCodes)
        {
            var id = Guid.NewGuid().ToString("N");

            var document = _project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var annotatedRoot = root!.WithAdditionalAnnotations(Simplifier.Annotation);

            var newDoc = document.WithSyntaxRoot(annotatedRoot);

            var simplifiedDoc = await Simplifier.ReduceAsync(newDoc, null, cancellationToken);

            yield return (await simplifiedDoc.GetTextAsync(cancellationToken)).ToString();
        }
    }
}