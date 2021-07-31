using System.Linq;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal record ProxyData(string Namespace, string InterfaceName, string RawTypeName, string TypeName, bool ProxyAll)
    {
        public string ClassName => RawTypeName.Split('.').Last();

        public string FileName => TypeName.Replace(".", "_");
    }
}