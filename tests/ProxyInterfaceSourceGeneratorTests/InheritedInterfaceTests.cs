using AwesomeAssertions;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Helpers;
using ProxyInterfaceSourceGeneratorTests.Source.Disposable;

namespace ProxyInterfaceSourceGeneratorTests;

public class InheritedInterfaceTests
{
    private const string Namespace = "ProxyInterfaceSourceGeneratorTests.Source.Disposable";
    private readonly ProxyInterfaceCodeGenerator _sut;
    private readonly string _basePath;
    private readonly string _outputPath;

    public InheritedInterfaceTests()
    {
        _basePath = TestHelper.TestProjectRoot.Value;
        _outputPath = Path.Combine(_basePath, "Destination/Disposable/");

        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }
        _sut = new ProxyInterfaceCodeGenerator();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void GenerateFiles_InheritedInterface_InheritFromBaseClass(bool proxyBaseClass, bool inheritBaseInterface)
    {
        var name = "Child";
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames =
        [
            $"{Namespace}.{interfaceName}.g.cs",
            $"{Namespace}.{proxyName}.g.cs"
        ];
        var path = Path.Combine(_basePath, $"Source/Disposable/{interfaceName}.cs");
        SourceFile sourceFile = CreateSourceFile(path, name, proxyBaseClass);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var interfaceIndex = 1;
        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToArray();

        // Assert
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList;
        bool didWeInherit = baseList is not null;
        Assert.Equal(didWeInherit, inheritBaseInterface);
    }

    [Theory]
    [InlineData("Parent")]
    [InlineData("Child")]
    public void GenerateFiles_InheritedInterface_Should_InheritTheInterface(string name)
    {
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames =
        [
            $"{Namespace}.{interfaceName}.g.cs",
            $"{Namespace}.{proxyName}.g.cs"
        ];

        var path = Path.Combine(_basePath, $"Source/Disposable/{interfaceName}.cs");
        SourceFile sourceFile = CreateSourceFile(path, name, true);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var interfaceIndex = 1;
        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToArray();

        // Assert
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList!;
        Assert.Equal(2, baseList.Types.Count);
        var type1 = (QualifiedNameSyntax)baseList.Types[0].Type;
        var type2 = (QualifiedNameSyntax)baseList.Types[1].Type;
        Assert.Equal(nameof(IDisposable), type1.Right.Identifier.Text);
        Assert.Equal(nameof(IUpdate<string>), type2.Right.Identifier.Text);
    }

    [Fact]
    public void GenerateFiles_InheritedInterface_Should_Not_InheritExplicitImplementedInterfaces()
    {
        var name = "Explicit";
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames =
        [
            $"{Namespace}.{interfaceName}.g.cs",
            $"{Namespace}.{proxyName}.g.cs"
        ];
        var interfaceIndex = 1;
        var path = Path.Combine(_basePath, $"Source/Disposable/{interfaceName}.cs");
        SourceFile sourceFile = CreateSourceFile(path, name, true);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToArray();

        // Assert
        //This actually could work, we just need to implement the logic inside the Proxy (and interface).
        //⚠ Dispose is not a public member of the 'Explicit' class and also not of the Proxy.
        //e.g. new Explicit().Dispose() is not possible.
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList;
        bool noInterfaceImplementationFound = baseList is null;
        Assert.True(noInterfaceImplementationFound);
    }

    private static SourceFile CreateSourceFile(string path, string name, bool extend)
    {
        var extendString = extend.ToString().ToLowerInvariant();

        return new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = $"typeof({Namespace}.{name}), {extendString}"
            }
        };
    }

    private void WriteFiles(string[] fileNames, ExecuteResult result)
    {
        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);
            File.WriteAllText(Path.Combine(_outputPath, fileName.fileName), builder.Text);
            builder.Text.Should().Be(File.ReadAllText(Path.Combine(_outputPath, fileName.fileName)));
        }
    }
}