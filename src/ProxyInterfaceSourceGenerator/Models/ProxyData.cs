using System.Text.RegularExpressions;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.Models;

internal class ProxyData
{
    public string Namespace { get; }

    public string NamespaceDot { get; }

    public string ShortInterfaceName { get; }

    public string FullInterfaceName { get; }

    public string FullQualifiedTypeName { get; }

    public string ShortMetadataName { get; }

    public string FullMetadataTypeName { get; }

    public List<string> Usings { get; }

    public bool ProxyBaseClasses { get; }

    public Regex[] MembersToIgnore { get; }

    public ProxyClassAccessibility Accessibility { get; }

    public ProxyData(
        string @namespace,
        string namespaceDot,
        string shortInterfaceName,
        string fullInterfaceName,
        string fullQualifiedTypeName,
        string shortMetadataTypeName,
        string fullMetadataTypeName,
        List<string> usings,
        bool proxyBaseClasses,
        ProxyClassAccessibility accessibility,
        string[] membersToIgnore)
    {
        Namespace = @namespace;
        NamespaceDot = namespaceDot;
        ShortInterfaceName = shortInterfaceName;
        FullInterfaceName = fullInterfaceName;
        FullQualifiedTypeName = fullQualifiedTypeName;
        ShortMetadataName = shortMetadataTypeName;
        FullMetadataTypeName = fullMetadataTypeName;
        Usings = usings;
        ProxyBaseClasses = proxyBaseClasses;
        Accessibility = accessibility;
        MembersToIgnore = CreateRegexArray(membersToIgnore);
    }

    private static Regex[] CreateRegexArray(IEnumerable<string> membersToIgnore)
    {
        return membersToIgnore
            .Select(member =>
            {
                return new Regex("^" + Regex.Escape(member).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                    RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            })
            .ToArray();
    }
}