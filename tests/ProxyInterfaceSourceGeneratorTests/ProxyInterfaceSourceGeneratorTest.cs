using System.Runtime.CompilerServices;
using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Helpers;
using ProxyInterfaceSourceGeneratorTests.Source;

namespace ProxyInterfaceSourceGeneratorTests;

[UsesVerify]
public class ProxyInterfaceSourceGeneratorTest
{
    [ModuleInitializer]
    public static void ModuleInitializer() => VerifySourceGenerators.Enable();

    private const bool Write = true;

    private readonly ProxyInterfaceCodeGenerator _sut;
    private readonly string _basePath;

    public ProxyInterfaceSourceGeneratorTest()
    {
        _sut = new ProxyInterfaceCodeGenerator();
        _basePath = TestHelper.TestProjectRoot.Value;
    }

    [Fact]
    public void GenerateFiles_ForStruct_Should_Not_GenerateProxyCode()
    {
        // Arrange
        var path = Path.Combine(_basePath, "Source/IMyStruct.cs");
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
        var result = _sut.Execute([sourceFile]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(1);
    }

    [Fact]
    public Task GenerateFiles_ForClassWithArray_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IFoo.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.FooProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IFoo.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.Foo)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        // Verify
        var results = result.GeneratorDriver.GetRunResult().Results.First().GeneratedSources;
        return Verify(results);
    }

    [Fact]
    public void GenerateFiles_ForGenericType_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IGeneric.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.Generic`1Proxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IGeneric.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.Generic<>)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }

    [Fact]
    public void GenerateFiles_ForÜberGenericType_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IÜberGeneric.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.ÜberGeneric`3Proxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IÜberGeneric.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.ÜberGeneric<>)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }

    [Fact]
    public void GenerateFiles_ForClassWithOperator_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IOperatorTest.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.OperatorTestProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IOperatorTest.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.OperatorTest)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }

        var name = "stef";
        var operatorTest = new OperatorTest
        {
            Name = name
        };
        string name1 = (string)operatorTest;
        name1.Should().Be(name);

        var p = new OperatorTestProxy(operatorTest);
        string name2 = (string)p;
        name2.Should().Be(name);

        var p2 = (OperatorTestProxy)name;
        p2.Should().BeEquivalentTo(new OperatorTestProxy(operatorTest));
    }

    [Fact]
    public void GenerateFiles_When_NoNamespace_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "INoNamespace.g.cs",
            "NoNamespaceProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/INoNamespace.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.NoNamespace)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }

    [Fact]
    public void GenerateFiles_When_MixedVisibility_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IMixedVisibility.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.MixedVisibilityProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IMixedVisibility.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(ProxyInterfaceSourceGeneratorTests.Source.MixedVisibility)"
            }
        };

        // Act
        var result = _sut.Execute([
            sourceFile
        ]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }

    [Fact]
    public void GenerateFiles_ForSingleClass_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceGenerator.Extra.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.IPersonExtends.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.PersonExtendsProxy.g.cs",
        };

        var path = Path.Combine(_basePath, "Source", "IPersonExtends.cs");
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
        var result = _sut.Execute([sourceFile]);

        // Assert
        Assert(result, fileNames, false);
    }

    [Fact]
    public void GenerateFiles_ForSingleClass_AsInternal_Should_GenerateCorrectFiles()
    {
        // Arrange
        var interfaceFilename = "ProxyInterfaceSourceGeneratorTests.Source.ITestClassInternal.g.cs";
        var proxyClassFilename = "ProxyInterfaceSourceGeneratorTests.Source.TestClassInternalProxy.g.cs";

        var path = Path.Combine(_basePath, "Source", "ITestClassInternal.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = new[] { "typeof(ProxyInterfaceSourceGeneratorTests.Source.TestClassInternal)", "ProxyClassAccessibility.Internal" }
            }
        };

        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(3);

        // Assert interface
        var @interface = result.Files[1].SyntaxTree;
        @interface.FilePath.Should().EndWith(interfaceFilename);

        var interfaceCode = @interface.ToString();
        var interfaceCodeFilename = Path.Combine(_basePath, "Destination", interfaceFilename);
        if (Write) File.WriteAllText(interfaceCodeFilename, interfaceCode);
        interfaceCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodeFilename));

        // Assert Proxy
        var proxyClass = result.Files[2].SyntaxTree;
        proxyClass.FilePath.Should().EndWith(proxyClassFilename);

        var proxyCode = proxyClass.ToString();
        var proxyCodeFilename = Path.Combine(_basePath, "Destination", proxyClassFilename);
        if (Write) File.WriteAllText(proxyCodeFilename, proxyCode);
        proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyCodeFilename));
    }

    [Fact]
    public void GenerateFiles_ForTwoClasses_Should_GenerateCorrectFiles()
    {
        // Arrange
        var attributeFilename = "ProxyInterfaceGenerator.Extra.g.cs";
        var interfaceHumanFilename = "ProxyInterfaceSourceGeneratorTests.Source.IHuman.g.cs";
        var proxyClassHumanFilename = "ProxyInterfaceSourceGeneratorTests.Source.HumanProxy.g.cs";
        var interfacePersonFilename = "ProxyInterfaceSourceGeneratorTests.Source.IPerson.g.cs";
        var proxyClassPersonFilename = "ProxyInterfaceSourceGeneratorTests.Source.PersonProxy.g.cs";

        var pathHuman = Path.Combine(_basePath, "Source", "IHuman.cs");
        var sourceFileHuman = new SourceFile
        {
            Path = pathHuman,
            Text = File.ReadAllText(pathHuman),
            AttributeToAddToInterface = "ProxyInterfaceGenerator.Proxy<ProxyInterfaceSourceGeneratorTests.Source.Human>"
        };

        var pathPerson = Path.Combine(_basePath, "Source", "IPerson.cs");
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

        // Act
        var result = _sut.Execute([sourceFileHuman, sourceFilePerson]);

        // Assert
        result.Valid.Should().BeTrue();
        result.WarningMessages.Should().BeEmpty();
        result.Files.Should().HaveCount(5);

        // Assert attribute
        var attribute = result.Files[0].SyntaxTree;
        attribute.FilePath.Should().EndWith(attributeFilename);


        // Assert interface Human
        var interfaceHuman = result.Files[1].SyntaxTree;
        interfaceHuman.FilePath.Should().EndWith(interfaceHumanFilename);

        var interfaceCodeHuman = interfaceHuman.ToString();
        var interfaceCodeHumanFilename = Path.Combine(_basePath, "Destination", interfaceHumanFilename);
        if (Write) File.WriteAllText(interfaceCodeHumanFilename, interfaceCodeHuman);
        interfaceCodeHuman.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodeHumanFilename));


        // Assert interface Person
        var interfacePerson = result.Files[2].SyntaxTree;
        interfacePerson.FilePath.Should().EndWith(interfacePersonFilename);

        var interfaceCodePerson = interfacePerson.ToString();
        var interfaceCodePersonFilename = Path.Combine(_basePath, "Destination", interfacePersonFilename);
        if (Write) File.WriteAllText(interfaceCodePersonFilename, interfaceCodePerson);
        interfaceCodePerson.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodePersonFilename));


        // Assert Proxy Human
        var proxyClassHuman = result.Files[3].SyntaxTree;
        proxyClassHuman.FilePath.Should().EndWith(proxyClassHumanFilename);

        var proxyCodeHuman = proxyClassHuman.ToString();
        var proxyCodeHumanFilename = Path.Combine(_basePath, "Destination", proxyClassHumanFilename);
        if (Write) File.WriteAllText(proxyCodeHumanFilename, proxyCodeHuman);
        proxyCodeHuman.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyCodeHumanFilename));


        // Assert Proxy Person
        var proxyClassPerson = result.Files[4].SyntaxTree;
        proxyClassPerson.FilePath.Should().EndWith(proxyClassPersonFilename);

        var proxyCode = proxyClassPerson.ToString();
        var proxyCodeFilename = Path.Combine(_basePath, "Destination", proxyClassPersonFilename);
        if (Write) File.WriteAllText(proxyCodeFilename, proxyCode);
        proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyCodeFilename));

        var personProxy = new PersonProxy(new Person());

        int c = 100;
        personProxy.In_Out_Ref1(1, out _, ref c);

        c.Should().Be(101);
    }

    [Fact]
    public void GenerateFiles_ForTwoClassesSameName_Should_GenerateCorrectFiles()
    {
        // Arrange
        var attributeFilename = "ProxyInterfaceGenerator.Extra.g.cs";

        var sourceFiles = new List<SourceFile>();

        var list = new[]
        {
            (IF: "", INS: "ProxyInterfaceDemo", I: "IGroup", CF: "", CNS: "ProxyInterfaceDemo", C: "Group"),
            (IF: "", INS: "ProxyInterfaceDemo", I: "IDisplayable", CF: "", CNS: "ProxyInterfaceDemo", C: "Displayable"),
            (IF: "", INS: "ProxyInterfaceDemo", I: "IDestroyable", CF: "", CNS: "ProxyInterfaceDemo", C: "Destroyable"),
            (IF: "Depth/", INS: "ProxyInterfaceDemo.Depth", I: "IGroupDepth", CF: "Depth/", CNS: "ProxyInterfaceDemo.Depth", C: "Group")
        };

        foreach (var x in list)
        {
            var pathInterface = Path.Combine(_basePath, "Source", $"{x.IF}{x.I}.cs");
            var sourceFile = new SourceFile
            {
                Path = pathInterface,
                Text = File.ReadAllText(pathInterface),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = $"typeof({x.CNS}.{x.C})"
                }
            };
            sourceFiles.Add(sourceFile);
        }

        // Act
        var result = _sut.Execute(sourceFiles);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(1 + list.Length * 2);

        // Assert attribute
        var attribute = result.Files[0].SyntaxTree;
        attribute.FilePath.Should().EndWith(attributeFilename);

        var fileIndex = 1;

        // Interfaces
        foreach (var x in list)
        {
            var filenameInterface = $"{x.INS}.{x.I}.g.cs";
            var syntaxTreeInterface = result.Files[fileIndex].SyntaxTree;
            syntaxTreeInterface.FilePath.Should().EndWith(filenameInterface);

            var codeInterface = syntaxTreeInterface.ToString();
            var path = Path.Combine(_basePath, "Destination", $"{x.IF}{filenameInterface}");
            if (Write) File.WriteAllText(path, codeInterface);
            codeInterface.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(path));

            fileIndex += 1;
        }

        // Proxies
        foreach (var x in list)
        {
            var filenameProxy = $"{x.CNS}.{x.C}Proxy.g.cs";
            var syntaxTreeProxy = result.Files[fileIndex].SyntaxTree;
            syntaxTreeProxy.FilePath.Should().EndWith(filenameProxy);

            var codeProxy = syntaxTreeProxy.ToString();
            var path = Path.Combine(_basePath, "Destination", $"{x.CF}{filenameProxy}");
            if (Write) File.WriteAllText(path, codeProxy);
            codeProxy.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(path));

            fileIndex += 1;
        }
    }

    [Fact]
    public void GenerateFiles_HttpClient()
    {
        // Arrange
        var attributeFilename = "ProxyInterfaceGenerator.Extra.g.cs";
        var interfaceIHttpClientFilename = "ProxyInterfaceSourceGeneratorTests.Source.IHttpClient.g.cs";
        var proxyClassIHttpClientFilename = "System.Net.Http.HttpClientProxy.g.cs";
        var interfaceIHttpMessageInvokerFilename = "ProxyInterfaceSourceGeneratorTests.Source.IHttpMessageInvoker.g.cs";
        var proxyClassIHttpMessageInvokerFilename = "System.Net.Http.HttpMessageInvokerProxy.g.cs";

        var pathIHttpClient = Path.Combine(_basePath, "Source", "IHttpClient.cs");
        var sourceFileIHttpClient = new SourceFile
        {
            Path = pathIHttpClient,
            Text = File.ReadAllText(pathIHttpClient),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(System.Net.Http.HttpClient)"
            }
        };

        var pathIHttpMessageInvoker = Path.Combine(_basePath, "Source", "IHttpMessageInvoker.cs");
        var sourceFileIHttpMessageInvoker = new SourceFile
        {
            Path = pathIHttpMessageInvoker,
            Text = File.ReadAllText(pathIHttpMessageInvoker),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(System.Net.Http.HttpMessageInvoker)"
            }
        };

        // Act
        var result = _sut.Execute([sourceFileIHttpClient, sourceFileIHttpMessageInvoker]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(5);

        // Assert attribute
        var attribute = result.Files[0].SyntaxTree;
        attribute.FilePath.Should().EndWith(attributeFilename);


        // Assert interface IHttpClient
        var interfaceIHttpClient = result.Files[1].SyntaxTree;
        interfaceIHttpClient.FilePath.Should().EndWith(interfaceIHttpClientFilename);

        var interfaceCodeIHttpClient = interfaceIHttpClient.ToString();
        var interfaceCodeIHttpClientFilename = Path.Combine(_basePath, "Destination", interfaceIHttpClientFilename);
        if (Write) File.WriteAllText(interfaceCodeIHttpClientFilename, interfaceCodeIHttpClient);
        interfaceCodeIHttpClient.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodeIHttpClientFilename));


        // Assert interface IHttpMessageInvoker
        var interfaceIMessageInvoker = result.Files[2].SyntaxTree;
        interfaceIMessageInvoker.FilePath.Should().EndWith(interfaceIHttpMessageInvokerFilename);

        var interfaceCodeIMessageInvoker = interfaceIMessageInvoker.ToString();
        var interfaceCodeIMessageInvokerFilename = Path.Combine(_basePath, "Destination", interfaceIHttpMessageInvokerFilename);
        if (Write) File.WriteAllText(interfaceCodeIMessageInvokerFilename, interfaceCodeIMessageInvoker);
        interfaceCodeIMessageInvoker.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodeIMessageInvokerFilename));


        // Assert Proxy IHttpClient
        var proxyClassIHttpClient = result.Files[3].SyntaxTree;
        proxyClassIHttpClient.FilePath.Should().EndWith(proxyClassIHttpClientFilename);

        var proxyCodeIHttpClient = proxyClassIHttpClient.ToString();
        var proxyCodeIHttpClientFilename = Path.Combine(_basePath, "Destination", proxyClassIHttpClientFilename);
        if (Write) File.WriteAllText(proxyCodeIHttpClientFilename, proxyCodeIHttpClient);
        proxyCodeIHttpClient.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyCodeIHttpClientFilename));


        // Assert Proxy IHttpMessageInvoker
        var proxyClassIMessageInvoker = result.Files[4].SyntaxTree;
        proxyClassIMessageInvoker.FilePath.Should().EndWith(proxyClassIHttpMessageInvokerFilename);

        var proxyIMessageInvoker = proxyClassIMessageInvoker.ToString();
        var proxyIMessageInvokerFilename = Path.Combine(_basePath, "Destination", proxyClassIHttpMessageInvokerFilename);
        if (Write) File.WriteAllText(proxyIMessageInvokerFilename, proxyIMessageInvoker);
        proxyIMessageInvoker.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyIMessageInvokerFilename));
    }

    [Fact]
    public void GenerateFiles_ForClassWithSameName_But_DifferentNamespace_Should_GenerateCorrectFiles()
    {
        // Arrange
        const string @class = "ClassInNamespace";
        foreach (var x in new[] { 1, 2 })
        {
            var attributeFilename = "ProxyInterfaceGenerator.Extra.g.cs";
            var interfaceFilename = $"ProxyInterfaceSourceGeneratorTests.Namespace{x}.I{@class}.g.cs";
            var proxyClassFilename = $"ProxyInterfaceSourceGeneratorTests.Namespace{x}.{@class}Proxy.g.cs";

            var path = Path.Combine(_basePath, "Source", $"I{@class}{x}.cs");
            var sourceFile = new SourceFile
            {
                Path = path,
                Text = File.ReadAllText(path),
                AttributeToAddToInterface = new ExtraAttribute
                {
                    Name = "ProxyInterfaceGenerator.Proxy",
                    ArgumentList = new[] { $"typeof(ProxyInterfaceSourceGeneratorTests.Namespace{x}.{@class})", "true" }
                }
            };

            // Act
            var result = _sut.Execute([sourceFile]);

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
            var interfaceCodeFilename = Path.Combine(_basePath, "Destination", interfaceFilename);
            if (Write) File.WriteAllText(interfaceCodeFilename, interfaceCode);
            interfaceCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(interfaceCodeFilename));

            // Assert Proxy
            var proxyClass = result.Files[2].SyntaxTree;
            proxyClass.FilePath.Should().EndWith(proxyClassFilename);

            var proxyCode = proxyClass.ToString();
            var proxyCodeFilename = Path.Combine(_basePath, "Destination", proxyClassFilename);
            if (Write) File.WriteAllText(proxyCodeFilename, proxyCode);
            proxyCode.Should().NotBeNullOrEmpty().And.Be(File.ReadAllText(proxyCodeFilename));
        }
    }

    [Fact]
    public Task GenerateFiles_ForClassWithIgnores()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IFoo2.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.Foo2Proxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IFoo2.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = new[]
                {
                    "typeof(ProxyInterfaceSourceGeneratorTests.Source.Foo2)", "false", "ProxyClassAccessibility.Public",
                    "new [] { \"Weird\", \"NotHere\" }"
                }
            }
        };

        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        // Verify
        var results = result.GeneratorDriver.GetRunResult().Results.First().GeneratedSources;
        return Verify(results);
    }

    [Fact]
    public Task GenerateFiles_ForClassWithIgnores_Regex()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.IFoo2.g.cs",
            "ProxyInterfaceSourceGeneratorTests.Source.Foo2Proxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/IFoo2.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = new[]
                {
                    "typeof(ProxyInterfaceSourceGeneratorTests.Source.Foo2)", "false", "ProxyClassAccessibility.Public",
                    "new [] { \"Weird*\" }"
                }
            }
        };

        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        // Verify
        var results = result.GeneratorDriver.GetRunResult().Results.First().GeneratedSources;
        return Verify(results);
    }

    [Fact]
    public void GenerateFiles_ForTimeProvider_Should_GenerateCorrectFiles()
    {
        // Arrange
        var fileNames = new[]
        {
            "ProxyInterfaceSourceGeneratorTests.Source.ITimeProvider.g.cs",
            "System.TimeProviderProxy.g.cs"
        };

        var path = Path.Combine(_basePath, "Source/ITimeProvider.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = "typeof(System.TimeProvider)"
            }
        };

        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        Assert(result, fileNames);
    }

    [Theory]
    [InlineData("ClassDirect")]
    [InlineData("ClassDirectAndIndirect")]
    public void GenerateFiles_Map(string value)
    {
        // Arrange
        var fileNames = new[]
        {
            $"ProxyInterfaceSourceGeneratorTests.Source.I{value}.g.cs",
            $"ProxyInterfaceSourceGeneratorTests.Source.{value}Proxy.g.cs"
        };

        var path = Path.Combine(_basePath, $"Source/I{value}.cs");
        var sourceFile = new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "ProxyInterfaceGenerator.Proxy",
                ArgumentList = $"typeof(ProxyInterfaceSourceGeneratorTests.Source.{value})"
            }
        };

        // Act
        var result = _sut.Execute([sourceFile]);

        // Assert
        Assert(result, fileNames);

        // Test
        var instance = new ClassDirectAndIndirect
        {
            Id = "Instance",
            Value = new ClassDirectAndIndirect { Id = "Value" },
            Array = [new ClassDirectAndIndirect { Id = "Array 1" }, new ClassDirectAndIndirect { Id = "Array 2" }],
            List = [new ClassDirectAndIndirect { Id = "List 1" }, new ClassDirectAndIndirect { Id = "List 2" }, new ClassDirectAndIndirect { Id = "List 3" }]
        };

        var proxy = new ClassDirectAndIndirectProxy(instance);
        proxy.Id.Should().Be("Instance");
        proxy.Value!.Id.Should().Be("Value");
        proxy.Array.Select(a => a.Id).Should().BeEquivalentTo(["Array 1", "Array 2"]);
        proxy.List.Select(a => a.Id).Should().BeEquivalentTo(["List 1", "List 2", "List 3"]);
    }

    private void Assert(ExecuteResult result, string[] fileNames, bool skipExtra = true)
    {
        var skip = skipExtra ? 1 : 0;

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + skip);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + skip]; // +1 means skip the attribute
            builder.Path.Should().EndWith(fileName.fileName);

            var destinationFilename = Path.Combine(_basePath, $"Destination/{fileName.fileName}");
            if (Write) File.WriteAllText(destinationFilename, builder.Text);
            builder.Text.Should().Be(File.ReadAllText(destinationFilename));
        }
    }
}