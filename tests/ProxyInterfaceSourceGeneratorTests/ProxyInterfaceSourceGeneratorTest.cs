using System.IO;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using Xunit;

namespace FluentBuilderGeneratorTests
{
    public class ProxyInterfaceSourceGeneratorTest
    {
        private readonly ProxyInterfaceCodeGenerator _sut;

        public ProxyInterfaceSourceGeneratorTest()
        {
            _sut = new ProxyInterfaceCodeGenerator();
        }

        [Fact]
        public void GenerateFiles_For1Class_Should_GenerateCorrectFiles()
        {
            // Arrange
            var proxyClassFilename = "ProxyInterfaceSourceGeneratorTests.DTO.PersonProxy.g.cs";
            var interfaceFilename = "ProxyInterfaceSourceGeneratorTests.DTO.IPerson.g.cs";

            var path = "./DTO/IPerson.cs";
            var sourceFile = new SourceFile
            {
                Path = path,
                Text = File.ReadAllText(path),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.DTO.Person)"
                }
            };

            // Act
            var result = _sut.Execute(new[] { sourceFile });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(3);

            // Assert interface
            var @interface = result.Files[1].SyntaxTree;
            @interface.FilePath.Should().EndWith(interfaceFilename);

            var interfaceCode = @interface.ToString();
            // File.WriteAllText($"../../../Result/{interfaceFilename}", interfaceCode);
            interfaceCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Result/{interfaceFilename}"));

            // Assert Proxy
            var proxyClass = result.Files[2].SyntaxTree;
            proxyClass.FilePath.Should().EndWith(proxyClassFilename);

            var proxyCode = proxyClass.ToString();
            // File.WriteAllText($"../../../Result/{proxyClassFilename}", proxyCode);
            proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Result/{proxyClassFilename}"));
        }
    }
}