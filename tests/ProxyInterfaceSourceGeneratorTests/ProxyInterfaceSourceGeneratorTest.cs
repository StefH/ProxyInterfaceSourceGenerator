using System.IO;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Source;
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

            var pp = new PersonProxy(new Person());

            var h = pp.AddHuman(new HumanProxy(new Human()));

            int x = 0;
        }

        [Fact]
        public void GenerateFiles_ForStruct_Should_Not_GenerateProxyCode()
        {
            // Arrange
            var path = "./Source/IMyStruct.cs";
            var sourceFile = new SourceFile
            {
                Path = path,
                Text = File.ReadAllText(path),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.MyStruct)"
                }
            };

            // Act
            var result = _sut.Execute(new[] { sourceFile });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(1);
        }

        [Fact]
        public void GenerateFiles_ForSingleClass_Should_GenerateCorrectFiles()
        {
            // Arrange
            var attributeFilename = "ProxyInterfaceGenerator.ProxyAttribute.g.cs";
            var interfaceFilename = "ProxyInterfaceSourceGeneratorTests.Source.IPersonExtends.g.cs";
            var proxyClassFilename = "ProxyInterfaceSourceGeneratorTests.Source.PersonExtendsProxy.g.cs";

            var path = "./Source/IPersonExtends.cs";
            var sourceFile = new SourceFile
            {
                Path = path,
                Text = File.ReadAllText(path),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = new[] { "typeof(ProxyInterfaceSourceGeneratorTests.Source.PersonExtends)", "true" }
                }
            };

            // Act
            var result = _sut.Execute(new[] { sourceFile });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(3);

            // Assert attribute
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

        [Fact]
        public void GenerateFiles_ForTwoClasses_Should_GenerateCorrectFiles()
        {
            // Arrange
            var attributeFilename = "ProxyInterfaceGenerator.ProxyAttribute.g.cs";
            var interfaceHumanFilename = "ProxyInterfaceSourceGeneratorTests.Source.IHuman.g.cs";
            var proxyClassHumanFilename = "ProxyInterfaceSourceGeneratorTests.Source.HumanProxy.g.cs";
            var interfacePersonFilename = "ProxyInterfaceSourceGeneratorTests.Source.IPerson.g.cs";
            var proxyClassPersonFilename = "ProxyInterfaceSourceGeneratorTests.Source.PersonProxy.g.cs";

            var pathPerson = "./Source/IPerson.cs";
            var sourceFilePerson = new SourceFile
            {
                Path = pathPerson,
                Text = File.ReadAllText(pathPerson),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.Person)"
                }
            };

            var pathHuman = "./Source/IHuman.cs";
            var sourceFileHuman = new SourceFile
            {
                Path = pathHuman,
                Text = File.ReadAllText(pathHuman),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.Human)"
                }
            };

            // Act
            var result = _sut.Execute(new[] { sourceFileHuman, sourceFilePerson });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(5);

            // Assert attribute
            var attribute = result.Files[0].SyntaxTree;
            attribute.FilePath.Should().EndWith(attributeFilename);


            // Assert interface Human
            var interfaceHuman = result.Files[1].SyntaxTree;
            interfaceHuman.FilePath.Should().EndWith(interfaceHumanFilename);

            var interfaceCodeHuman = interfaceHuman.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfaceHumanFilename}", interfaceCodeHuman);
            interfaceCodeHuman.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfaceHumanFilename}"));


            // Assert interface Person
            var interfacePerson = result.Files[2].SyntaxTree;
            interfacePerson.FilePath.Should().EndWith(interfacePersonFilename);

            var interfaceCodePerson = interfacePerson.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfacePersonFilename}", interfaceCodePerson);
            interfaceCodePerson.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfacePersonFilename}"));


            // Assert Proxy Human
            var proxyClassHuman = result.Files[3].SyntaxTree;
            proxyClassHuman.FilePath.Should().EndWith(proxyClassHumanFilename);

            var proxyCodeHuman = proxyClassHuman.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassHumanFilename}", proxyCodeHuman);
            proxyCodeHuman.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassHumanFilename}"));


            // Assert Proxy Person
            var proxyClassPerson = result.Files[4].SyntaxTree;
            proxyClassPerson.FilePath.Should().EndWith(proxyClassPersonFilename);

            var proxyCode = proxyClassPerson.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassPersonFilename}", proxyCode);
            proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassPersonFilename}"));
        }

        [Fact]
        public void GenerateFiles_ForPnP_Should_GenerateCorrectFiles()
        {
            // Arrange
            var interfaceWebFilename = "ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb.g.cs";
            var proxyClassWebFilename = "Microsoft.SharePoint.Client.WebProxy.g.cs";
            var interfaceClientContextFilename = "ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext.g.cs";
            var proxyClassClientContextFilename = "Microsoft.SharePoint.Client.ClientContextProxy.g.cs";
            var interfaceClientRuntimeContextFilename = "ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext.g.cs";
            var proxyClassClientRuntimeContextFilename = "Microsoft.SharePoint.Client.ClientRuntimeContextProxy.g.cs";

            var pathWeb = "./Source/PnP/IWeb.cs";
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

            var pathClientContext = "./Source/PnP/IClientContext.cs";
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

            var pathClientRuntimeContext = "./Source/Pnp/IClientRuntimeContext.cs";
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

            // Act
            var result = _sut.Execute(new[] { sourceFileWeb, sourceFileClientRuntimeContext, sourceFileClientContext });

            // Assert
            result.Valid.Should().BeTrue();
            result.Files.Should().HaveCount(7);

            // Assert interface Web
            var interfaceWeb = result.Files[1].SyntaxTree;
            interfaceWeb.FilePath.Should().EndWith(interfaceWebFilename);

            var interfaceCodeWeb = interfaceWeb.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfaceWebFilename}", interfaceCodeWeb);
            interfaceCodeWeb.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfaceWebFilename}"));


            // Assert interface ClientRuntimeContext
            var interfaceClientRuntimeContext = result.Files[2].SyntaxTree;
            interfaceClientRuntimeContext.FilePath.Should().EndWith(interfaceClientRuntimeContextFilename);

            var interfaceCodeClientRuntimeContext = interfaceClientRuntimeContext.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfaceClientRuntimeContextFilename}", interfaceCodeClientRuntimeContext);
            interfaceCodeClientRuntimeContext.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfaceClientRuntimeContextFilename}"));


            // Assert interface ClientContext
            var interfaceClientContext = result.Files[3].SyntaxTree;
            interfaceClientContext.FilePath.Should().EndWith(interfaceClientContextFilename);

            var interfaceCodeClientContext = interfaceClientContext.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{interfaceClientContextFilename}", interfaceCodeClientContext);
            interfaceCodeClientContext.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{interfaceClientContextFilename}"));


            // Assert Proxy Web
            var proxyClassWeb = result.Files[4].SyntaxTree;
            proxyClassWeb.FilePath.Should().EndWith(proxyClassWebFilename);

            var proxyCodeWeb = proxyClassWeb.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassWebFilename}", proxyCodeWeb);
            proxyCodeWeb.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassWebFilename}"));


            // Assert Proxy ClientRuntimeContext
            var proxyClassClientRuntimeContext = result.Files[5].SyntaxTree;
            proxyClassClientRuntimeContext.FilePath.Should().EndWith(proxyClassClientRuntimeContextFilename);

            var proxyCodeClientRuntimeContext = proxyClassClientRuntimeContext.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassClientRuntimeContextFilename}", proxyCodeClientRuntimeContext);
            proxyCodeClientRuntimeContext.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassClientRuntimeContextFilename}"));

            // Assert Proxy ClientContext
            var proxyClassClientContext = result.Files[6].SyntaxTree;
            proxyClassClientContext.FilePath.Should().EndWith(proxyClassClientContextFilename);

            var proxyCodeClientContext = proxyClassClientContext.ToString();
            if (Write) File.WriteAllText($"../../../Destination/{proxyClassClientContextFilename}", proxyCodeClientContext);
            proxyCodeClientContext.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText($"../../../Destination/{proxyClassClientContextFilename}"));
        }
    }
}