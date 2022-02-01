using System.IO;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using Xunit;

namespace ProxyInterfaceSourceGeneratorTests
{
    public class ProxyInterfaceSourceGeneratorTest
    {
        private bool Write = true;

        private readonly ProxyInterfaceCodeGenerator _sut;

        public ProxyInterfaceSourceGeneratorTest()
        {
            _sut = new ProxyInterfaceCodeGenerator();
        }

        [Fact]
        public void GenerateFiles_ForSingleClass_Should_GenerateCorrectFiles()
        {
            // Arrange
            var attributeFilename = "ProxyInterfaceGenerator.ProxyAttribute.g.cs";
            var interfaceFilename = "ProxyInterfaceSourceGeneratorTests.Source.IPerson.g.cs";
            var proxyClassFilename = "ProxyInterfaceSourceGeneratorTests.Source.PersonProxy.g.cs";

            var path = "./Source/IPerson.cs";
            var sourceFile = new SourceFile
            {
                Path = path,
                Text = File.ReadAllText(path),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.Person)"
                }
            };

            // Act
            var result = _sut.Execute(new[] { sourceFile });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(3);

            // Assert interface
            var attribute = result.Files[0].SyntaxTree;
            attribute.FilePath.Should().EndWith(attributeFilename);

            // Assert interface
            var @interface = result.Files[1].SyntaxTree;
            @interface.FilePath.Should().EndWith(interfaceFilename);

            var interfaceCode = @interface.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfaceFilename}", interfaceCode);
            interfaceCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfaceFilename}"));

            // Assert Proxy
            var proxyClass = result.Files[2].SyntaxTree;
            proxyClass.FilePath.Should().EndWith(proxyClassFilename);

            var proxyCode = proxyClass.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassFilename}", proxyCode);
            proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassFilename}"));
        }
    }
}