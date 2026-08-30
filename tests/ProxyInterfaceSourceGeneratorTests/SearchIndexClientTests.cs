using AwesomeAssertions;
//using Azure.Search.Documents.Indexes;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Helpers;

namespace ProxyInterfaceSourceGeneratorTests;

public class SearchIndexClientTests
{
    private bool Write = true;

    private readonly ProxyInterfaceCodeGenerator _sut;
    private readonly string _basePath;

    public SearchIndexClientTests()
    {
        _sut = new ProxyInterfaceCodeGenerator();
        _basePath = TestHelper.TestProjectRoot.Value;
    }

    [Fact]
    public void GenerateFiles_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.AzureSearch.IAzureSearchIndexClient.g.cs",
            "Azure.Search.Documents.Indexes.SearchIndexClientProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/AzureSearch/IAzureSearchIndexClient.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(Azure.Search.Documents.Indexes.SearchIndexClient)"
            }
        };
        Azure.Search.Documents.Indexes.SearchIndexClient x;
        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);


            var destinationFilename = Path.Combine(_basePath, $"Destination/AzureSearchGenerated/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }
}