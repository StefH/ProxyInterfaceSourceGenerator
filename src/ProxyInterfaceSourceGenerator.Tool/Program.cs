using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGenerator.Tool;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddCommandLine(args);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<Generator>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

using var host = builder.Build();

//var logger = host.Services.GetRequiredService<ILogger<Program>>();

var generator = host.Services.GetRequiredService<Generator>();

generator.Generate();

return;

IEnumerable<PortableExecutableReference> referencesold = from a in AppDomain.CurrentDomain.GetAssemblies()
    where !a.IsDynamic
    select MetadataReference.CreateFromFile(a.Location);

// C:\dev\GitHub\ProxyInterfaceSourceGenerator\src-examples\ClassLibraryExampleForTool\bin\Debug\net8.0\ClassLibraryExampleForTool.dll

var dlls = Directory.EnumerateFiles(@"C:\dev\GitHub\ProxyInterfaceSourceGenerator\src-examples\ClassLibraryExampleForTool\bin\Debug\net8.0", "*.dll");
IEnumerable<PortableExecutableReference> references = dlls.Select(d => MetadataReference.CreateFromFile(d));

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText("TextFile1.txt"));

CSharpCompilation compilation = CSharpCompilation.Create("GeneratedNamespace_" + Guid.NewGuid().ToString().Replace("-", ""), [syntaxTree], references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));



Compilation outputCompilation;
ImmutableArray<Diagnostic> diagnostics;
var generatorDriver = CSharpGeneratorDriver.Create(new ProxyInterfaceCodeGenerator())
    //.AddAdditionalTexts(ImmutableArray.CreateRange(items))
    .RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out diagnostics);


var files = outputCompilation.SyntaxTrees.Skip(1).ToArray();

foreach (var file in files)
{
    File.WriteAllText(Path.GetFileName(file.FilePath), file.ToString());
}