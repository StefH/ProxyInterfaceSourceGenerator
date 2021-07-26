using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ProxyInterfaceSourceGenerator.FileGenerators;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;

namespace ProxyInterfaceSourceGenerator
{
    [Generator]
    public class ProxyInterfaceCodeGenerator : ISourceGenerator
    {
        private readonly ProxyAttributeGenerator _proxyAttributeGenerator = new ProxyAttributeGenerator();

        public void Initialize(GeneratorInitializationContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }

            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext ctx)
        {
            if (ctx.SyntaxReceiver is not ProxySyntaxReceiver receiver)
            {
                return;
            }

            var context1 = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };

            var attributeData = _proxyAttributeGenerator.GenerateFile();
            context1.GeneratorExecutionContext.AddSource(attributeData.FileName, SourceText.From(attributeData.Text, Encoding.UTF8));

            var context2 = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };
            var partialInterfacesGenerator = new PartialInterfacesGenerator(context2);
            foreach (var data in partialInterfacesGenerator.GenerateFiles())
            {
                context2.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }

            var context3 = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };
            var proxyClassesGenerator = new ProxyClassesGenerator(context3);
            foreach (var data in proxyClassesGenerator.GenerateFiles())
            {
                context3.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }
        }
    }
}