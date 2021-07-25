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
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext ctx)
        {
            if (ctx.SyntaxReceiver is not ProxySyntaxReceiver receiver)
            {
                return;
            }

            var context = new Context
            {
                GeneratorExecutionContext = ctx,
                CandidateInterfaces = receiver.CandidateInterfaces
            };

            var attributeData = _proxyAttributeGenerator.GenerateFile();
            context.GeneratorExecutionContext.AddSource(attributeData.FileName, SourceText.From(attributeData.Text, Encoding.UTF8));

            var partialInterfacesGenerator = new PartialInterfacesGenerator(context);
            foreach (var data in partialInterfacesGenerator.GenerateFiles())
            {
                context.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }

            var classesGenerator = new ProxyClassesGenerator(context);
            foreach (var data in classesGenerator.GenerateFiles())
            {
                context.GeneratorExecutionContext.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }
        }
    }
}