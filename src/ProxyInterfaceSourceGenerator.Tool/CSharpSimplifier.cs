using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ProxyInterfaceSourceGenerator.Tool
{
    public static class CSharpSimplifier
    {
        public static async Task<string> SimplifyCSharpCodeAsync(string sourceCode)
        {
            var id = Guid.NewGuid().ToString("N");

            var workspace = new AdhocWorkspace();
            workspace.Options
                .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 2);
                //.WithChangedOption(new OptionKey(SimplificationOptions., LanguageNames.CSharp), );

            var project = workspace
                .CurrentSolution
                .AddProject(id, $"{id}.dll", LanguageNames.CSharp)
                .WithMetadataReferences(GetDefaultReferences());

            var document = project.AddDocument($"Input_{id}.cs", SourceText.From(sourceCode));

            // Annotate document to allow simplification
            var annotatedDoc = await Simplifier.ReduceAsync(document, workspace.Options);

            // Format the document after simplification
            var formattedDoc = await Formatter.FormatAsync(annotatedDoc, workspace.Options);

            var simplifiedText = await formattedDoc.GetTextAsync();
            return simplifiedText.ToString();
        }

        private static IEnumerable<MetadataReference> GetDefaultReferences()
        {
            return
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)
            ];
        }
    }
}