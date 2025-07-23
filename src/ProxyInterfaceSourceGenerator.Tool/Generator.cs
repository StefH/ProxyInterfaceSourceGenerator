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
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var references = PortableExecutableReferenceUtils.GetAllReferences(_sourceDll);

        var allText = File.ReadAllText(_sourceFile);

        var existingNames = new HashSet<string>();

        foreach (var textPart in allText.Split("\r\n\r\n").Where(t => t.StartsWith("namespace")))
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(textPart);

            var compilation = CSharpCompilation.Create(
                "GeneratedNamespace_" + Guid.NewGuid().ToString().Replace("-", ""),
                [syntaxTree],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            _ = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator())
                .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var files = outputCompilation.SyntaxTrees.Skip(2).ToArray();

            foreach (var file in files)
            {
                var filename = GetUniqueTrimmedFileName(file.FilePath, existingNames);
                Console.WriteLine($"Writing file: {filename}");
                File.WriteAllText(Path.Combine(_outputPath, filename), file.ToString());
            }
        }
    }

    public static string GetUniqueTrimmedFileName(string fullFileName, HashSet<string> existingNames)
    {
        // Extract the last part (after last '.')
        var fileName = Path.GetFileName(fullFileName);
        var parts = fileName.Split('.');
        if (parts.Length > 3)
        {
            // Get the last 3 parts: Name + 'g' + Extension
            fileName = $"{parts[^3]}.{parts[^2]}.{parts[^1]}";
        }

        // Ensure uniqueness
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var finalName = fileName;
        int counter = 1;

        while (existingNames.Contains(finalName))
        {
            finalName = $"{baseName}_{counter}{extension}";
            counter++;
        }

        existingNames.Add(finalName);
        return finalName;
    }
}