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
            //if (!System.DiagnosticsDebugger.IsAttached)
            //{
            //    System.DiagnosticsDebugger.Launch();
            //}

            context.RegisterForSyntaxNotifications(() => new ProxySyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var attributeData = _proxyAttributeGenerator.GenerateFile();
            context.AddSource(attributeData.FileName, SourceText.From(attributeData.Text, Encoding.UTF8));

            if (context.SyntaxReceiver is not ProxySyntaxReceiver receiver)
            {
                return;
            }

            var partialInterfacesGenerator = new PartialInterfacesGenerator(context, receiver.CandidateInterfaces);
            foreach (var data in partialInterfacesGenerator.GenerateFiles())
            {
                context.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }

            var classesGenerator = new ProxyClassesGenerator(context, receiver.CandidateInterfaces);
            foreach (var data in classesGenerator.GenerateFiles())
            {
                context.AddSource(data.FileName, SourceText.From(data.Text, Encoding.UTF8));
            }
        }
    }

    public struct Test
    {
        public int Id { get; set; }

        public Clazz C { get; }

        public int Add(string s)
        {
            return 600;
        }
    }

    public sealed class Clazz
    {
        public string Name { get; set; }
    }

    public interface ITest
    {
        int Id { get; set; }

        IClazz C { get; }

        int Add(string s);
    }

    public interface IClazz
    {
        string Name { get; set; }
    }

    public class TestMock : ITest
    {
        private Test _instance;

        private IClazz _clazz;

        public TestMock(Test instance)
        {
            _instance = instance;

            _clazz = new ClazzMock(_instance.C);
        }

        public int Id
        {
            get => _instance.Id;
            set => _instance.Id = value;
        }

        public IClazz C => _clazz;

        public int Add(string s) => _instance.Add(s);
    }

    public class ClazzMock : IClazz
    {
        private Clazz _instance;

        public ClazzMock(Clazz instance)
        {
            _instance = instance;
        }

        public string Name { get => _instance.Name;  set => _instance.Name = value; }
    }
}