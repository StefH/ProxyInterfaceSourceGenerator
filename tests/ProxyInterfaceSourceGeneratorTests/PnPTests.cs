using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Helpers;

namespace ProxyInterfaceSourceGeneratorTests;

public class PnPTests
{
    private bool Write = true;

    private readonly ProxyInterfaceCodeGenerator _sut;
    private readonly string _basePath;

    public PnPTests()
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
            "ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext.g.cs",

            "Microsoft.SharePoint.Client.ClientObjectProxy.g.cs",
            "Microsoft.SharePoint.Client.SecurableObjectProxy.g.cs",
            "Microsoft.SharePoint.Client.WebProxy.g.cs",
            "Microsoft.SharePoint.Client.ClientRuntimeContextProxy.g.cs",
            "Microsoft.SharePoint.Client.ClientContextProxy.g.cs"
        };

        var pathClientObject = "Source/PnP/IClientObject.cs";
        var sourceFileClientObject = new SourceFile
        {
            Path = pathClientObject,
            Text = File.ReadAllText(pathClientObject),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(Microsoft.SharePoint.Client.ClientObject)"
            }
        };

        var pathSec = "Source/PnP/ISecurableObject.cs";
        var sourceFileSec = new SourceFile
        {
            Path = pathSec,
            Text = File.ReadAllText(pathSec),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(SecurableObject)" // Only name, no namespace
            }
        };

        var pathWeb = "Source/PnP/IWeb.cs";
        var sourceFileWeb = new SourceFile
        {
            Path = pathWeb,
            Text = File.ReadAllText(pathWeb),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(Web)" // Only name, no namespace
            }
        };

        var pathClientRuntimeContext = "Source/Pnp/IClientRuntimeContext.cs";
        var sourceFileClientRuntimeContext = new SourceFile
        {
            Path = pathClientRuntimeContext,
            Text = File.ReadAllText(pathClientRuntimeContext),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(Microsoft.SharePoint.Client.ClientRuntimeContext)"
            }
        };

        var pathClientContext = "Source/PnP/IClientContext.cs";
        var sourceFileClientContext = new SourceFile
        {
            Path = pathClientContext,
            Text = File.ReadAllText(pathClientContext),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ClientContext)" // Only name, no namespace
            }
        };

        // Act
        var result = _sut.Execute(
        [
            sourceFileClientObject,
            sourceFileSec,
            sourceFileWeb,
            sourceFileClientRuntimeContext,
            sourceFileClientContext
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFileName = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFileName, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFileName));
        }
    }
}