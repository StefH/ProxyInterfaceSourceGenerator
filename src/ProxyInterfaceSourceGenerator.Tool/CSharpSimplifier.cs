using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
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
        //workspace.Options
        //    .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 2);
            //.WithChangedOption(new OptionKey(SimplificationOptions., LanguageNames.CSharp), );

        var project = workspace
            .CurrentSolution
            .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
            .WithMetadataReferences(_references);

        var document = project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

        // Annotate document to allow simplification
        var annotatedDoc = await Simplifier.ReduceAsync(document, workspace.Options);

        // Format the document after simplification
        var formattedDoc = await Formatter.FormatAsync(annotatedDoc, workspace.Options);

        var simplifiedText = await formattedDoc.GetTextAsync();
        return simplifiedText.ToString();
    }
}