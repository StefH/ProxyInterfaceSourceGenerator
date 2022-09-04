using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProxyInterfaceSourceGenerator.FileGenerators;
using ProxyInterfaceSourceGenerator.Models;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;

namespace ProxyInterfaceSourceGenerator;

[Generator]
internal class ProxyInterfaceCodeGenerator : ISourceGenerator
{
    private readonly ProxyAttributeGenerator _proxyAttributeGenerator = new ();

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUGATTACH
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif

        context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            if (context.ParseOptions is not CSharpParseOptions csharpParseOptions)
            {
                throw new NotSupportedException("Only C# is supported.");
            }

            if (context.SyntaxReceiver is not ProxySyntaxReceiver receiver)
            {
                throw new NotSupportedException($"Only {nameof(ProxySyntaxReceiver)} is supported.");
            }

            // https://github.com/reactiveui/refit/blob/main/InterfaceStubGenerator.Core/InterfaceStubGenerator.cs
            var supportsNullable = csharpParseOptions.LanguageVersion >= LanguageVersion.CSharp8;

            GenerateProxyAttribute(context, receiver);
            GeneratePartialInterfaces(context, receiver, supportsNullable);
            GenerateProxyClasses(context, receiver, supportsNullable);
        }
        catch (Exception exception)
        {
            GenerateError(context, exception);
        }
    }

    private void GenerateError(GeneratorExecutionContext context, Exception exception)
    {
        var message = $"/*\r\n{nameof(ProxyInterfaceCodeGenerator)}\r\n\r\n[Exception]\r\n{exception}\r\n\r\n[StackTrace]\r\n{exception.StackTrace}*/";
        context.AddSource("Error.g", SourceText.From(message, Encoding.UTF8));
    }

    private void GenerateProxyAttribute(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver)
    {
        var context = new Context
        {
            GeneratorExecutionContext = ctx,
            Candidates = receiver.CandidateInterfaces
        };

        var attributeData = _proxyAttributeGenerator.GenerateFile();
        context.GeneratorExecutionContext.AddSource(attributeData.FileName, SourceText.From(attributeData.Text, Encoding.UTF8));
    }

    private static void GeneratePartialInterfaces(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver, bool supportsNullable)
    {
        var context = new Context
        {
            GeneratorExecutionContext = ctx,
            Candidates = receiver.CandidateInterfaces
        };

        var partialInterfacesGenerator = new PartialInterfacesGenerator(context, supportsNullable);
        foreach (var (fileName, text) in partialInterfacesGenerator.GenerateFiles())
        {
            context.GeneratorExecutionContext.AddSource(fileName, SourceText.From(text, Encoding.UTF8));
        }
    }

    private static void GenerateProxyClasses(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver, bool supportsNullable)
    {
        var context = new Context
        {
            GeneratorExecutionContext = ctx,
            Candidates = receiver.CandidateInterfaces
        };

        var proxyClassesGenerator = new ProxyClassesGenerator(context, supportsNullable);
        foreach (var (fileName, text) in proxyClassesGenerator.GenerateFiles())
        {
            context.GeneratorExecutionContext.AddSource(fileName, SourceText.From(text, Encoding.UTF8));
        }
    }
}