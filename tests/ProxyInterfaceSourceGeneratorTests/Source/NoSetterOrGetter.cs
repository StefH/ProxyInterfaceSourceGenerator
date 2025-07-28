namespace ProxyInterfaceSourceGeneratorTests.Source;

public class NoSetterOrGetter
{
    public Bar? BarNoSetter { get; }

    public Bar2? Bar2NoPublicGetter { set; private get; }
}