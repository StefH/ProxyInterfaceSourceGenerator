namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal record ProxyData(string Namespace, string InterfaceName, string RawTypeName, string TypeName, bool ProxyAll)
    {
        public string FileName => TypeName.Replace('.', '_').Replace('`', '_');
    }
}