using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public class CSharpSimplifier(HashSet<MetadataReference> references)
{
    public async Task<string> SimplifyCSharpCodeAsync(string sourceCode)
    {
        var id = Guid.NewGuid().ToString("N");

        using var workspace = new AdhocWorkspace();

        var project = workspace
            .CurrentSolution
            .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
            .WithMetadataReferences(references);

        var document = project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

        var root = await document.GetSyntaxRootAsync();

        var annotatedRoot = root!.WithAdditionalAnnotations(Simplifier.Annotation);

        var newDoc = document.WithSyntaxRoot(annotatedRoot);

        var simplifiedDoc = await Simplifier.ReduceAsync(newDoc, workspace.Options);

        return (await simplifiedDoc.GetTextAsync()).ToString();
    }
}