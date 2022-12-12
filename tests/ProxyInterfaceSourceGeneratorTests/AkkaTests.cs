using System.IO;
using System.Linq;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using Xunit;

namespace ProxyInterfaceSourceGeneratorTests;

public class AkkaTests
{
    private bool Write = true;

    private readonly ProxyInterfaceCodeGenerator _sut;

    public AkkaTests()
    {
        _sut = new ProxyInterfaceCodeGenerator();
    }

    [Fact]
    public void GenerateFiles_Should_GenerateCorrectFiles()
    {
        // Arrange
        Akka.Actor.LocalActorRefProvider a;

        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.AkkaActor.ILocalActorRefProvider.g.cs",
            "Akka.Actor.LocalActorRefProviderProxy.g.cs"
        };

        var path = "./Source/AkkaActor/ILocalActorRefProvider.cs";
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(Akka.Actor.LocalActorRefProvider)"
            }
        };

        // Act
        var result = _sut.Execute(new[]
        {
            sourceFile
        });

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            if (Write) File.WriteAllText($"../../../Destination/AkkaGenerated/{fileName.fileName}", builder.Text);
            builder.Text.Should().Be(File.ReadAllText($"../../../Destination/AkkaGenerated/{fileName.fileName}"));
        }
    }
}