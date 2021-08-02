using System.Collections.Generic;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal record ProxyData(string Namespace, string InterfaceName, string RawTypeName, string TypeName, List<string> Usings,  bool ProxyAll)
    {
        public string FileName => TypeName.Replace('.', '_').Replace('`', '_');
    }
}