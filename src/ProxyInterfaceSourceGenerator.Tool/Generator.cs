using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

    public void Generate()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(_sourceFile));

        var references = PortableExecutableReferenceUtils.GetAllReferences(_sourceDll);
        
        var compilation = CSharpCompilation.Create(
            "GeneratedNamespace_" + Guid.NewGuid().ToString().Replace("-", ""),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var files = outputCompilation.SyntaxTrees.Skip(1).ToArray();

        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        foreach (var file in files)
        {
            File.WriteAllText(Path.Combine(_outputPath, Path.GetFileName(file.FilePath)), file.ToString());
        }
    }
}