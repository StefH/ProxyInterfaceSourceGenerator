namespace ProxyInterfaceSourceGenerator.Models;

internal class ProxyData
{
    public string Namespace { get; init; }
    public string ShortInterfaceName { get; init; }
    public string FullInterfaceName { get; init; }
    public string FullRawTypeName { get; set; }
    public string ShortTypeName { get; init; }
    public string FullTypeName { get; init; }
    public List<string> Usings { get; init; }
    public bool ProxyBaseClasses { get; init; }
}