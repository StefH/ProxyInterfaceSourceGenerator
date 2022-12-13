namespace ProxyInterfaceSourceGeneratorTests.Source;

public class MixedVisibility
{
    public string Foo { get; protected set; } //<- this will generate bad code
}