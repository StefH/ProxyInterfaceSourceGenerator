using System.Linq;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal record ProxyData(string Namespace, string InterfaceName, string TypeName, bool ProxyAll)
    {
        public string ClassName => TypeName.Split('.').Last();
    }
}