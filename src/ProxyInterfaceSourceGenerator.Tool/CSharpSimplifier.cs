using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public class CSharpSimplifier
{
    private readonly HashSet<MetadataReference> _references;

    public CSharpSimplifier(HashSet<MetadataReference> references)
    {
        _references = references;
    }

    public async Task<string> SimplifyCSharpCodeAsync(string sourceCode)
    {
        var id = Guid.NewGuid().ToString("N");

        var workspace = new AdhocWorkspace();
        workspace.Options
            .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 2);           

        var project = workspace
            .CurrentSolution
            .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
            .WithMetadataReferences(_references);

        var document = project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

        var root = await document.GetSyntaxRootAsync();

        var annotatedRoot = root!.WithAdditionalAnnotations(Simplifier.Annotation);

        var newDoc = document.WithSyntaxRoot(annotatedRoot);

        var simplifiedDoc = await Simplifier.ReduceAsync(newDoc, workspace.Options);

        var formattedDoc = await Formatter.FormatAsync(simplifiedDoc, workspace.Options);

        var simplifiedCode = (await formattedDoc.GetTextAsync()).ToString();

        return simplifiedCode;
    }
}