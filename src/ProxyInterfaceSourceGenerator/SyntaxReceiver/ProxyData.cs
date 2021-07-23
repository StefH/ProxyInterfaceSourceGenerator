namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal record ProxyData
    {
        public string TypeName { get; init; }

        public bool ProxyAll { get; init; }
    }
}