using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ProxyInterfaceSourceGenerator.FileGenerators;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;

namespace ProxyInterfaceSourceGenerator
{
    [Generator]
    internal class ProxyInterfaceCodeGenerator : ISourceGenerator
    {
        private readonly ProxyAttributeGenerator _proxyAttributeGenerator = new ProxyAttributeGenerator();

        public void Initialize(GeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ProxySyntaxReceiver receiver)
            {
                return;
            }

            GenerateProxyAttribute(context, receiver);
            GeneratePartialInterfaces(context, receiver);
            GenerateProxyClasses(context, receiver);
        }

        private void GenerateProxyAttribute(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver)
        {
            var context = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };

            var attributeData = _proxyAttributeGenerator.GenerateFile();
            context.GeneratorExecutionContext.AddSource(attributeData.FileName, SourceText.From(attributeData.Text, Encoding.UTF8));
        }

        private static void GeneratePartialInterfaces(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver)
        {
            var context = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };

            var partialInterfacesGenerator = new PartialInterfacesGenerator(context);
            foreach (var data in partialInterfacesGenerator.GenerateFiles())
            {
                context.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }
        }

        private static void GenerateProxyClasses(GeneratorExecutionContext ctx, ProxySyntaxReceiver receiver)
        {
            var context = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };

            var proxyClassesGenerator = new ProxyClassesGenerator(context);
            foreach (var data in proxyClassesGenerator.GenerateFiles())
            {
                context.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }
        }
    }
}