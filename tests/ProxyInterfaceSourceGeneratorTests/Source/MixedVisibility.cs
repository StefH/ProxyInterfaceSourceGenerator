using System.Diagnostics.CodeAnalysis;

namespace ProxyInterfaceSourceGeneratorTests.Source;

public class MixedVisibility
{
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public string Foo { get; protected set; } = null!;//<- this will generate bad code
}
