using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.Extensions.Configuration;

namespace ProxyInterfaceSourceGenerator.Tool;

internal class Generator
{
    private readonly string _sourceDll;
    private readonly string _sourceFile;
    private readonly string _outputPath;

    public Generator(IConfiguration configuration)
    {
        _sourceDll = configuration["sourceDll"] ?? throw new ArgumentNullException();
        _sourceFile = configuration["sourceFile"] ?? throw new ArgumentNullException();
        _outputPath = configuration["outputPath"] ?? ".";
    }

    public async Task GenerateAsync()
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var references = MetadataReferenceUtils.GetAllReferences(_sourceDll);

        var allText = File.ReadAllText(_sourceFile);

        var syntaxTree = CSharpSyntaxTree.ParseText(allText);

        var compilation = CSharpCompilation.Create(
            "GeneratedNamespace_" + Guid.NewGuid().ToString().Replace("-", ""),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator(GenerateFileAction)).RunGeneratorsAndUpdateCompilation(compilation, out _, out _);
    }

    private void GenerateFileAction(string fileName, string content)
    {
        var fullPath = Path.Combine(_outputPath, fileName);
        Console.WriteLine($"Writing file: {fullPath}");

        var modified = "";
        CSharpSimplifier.SimplifyCSharpCodeAsync(content).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Console.WriteLine($"Error simplifying code: {task.Exception?.Message}");
                return;
            }
            modified = task.Result;
        }).Wait();

        File.WriteAllText(fullPath, modified);
    }
}