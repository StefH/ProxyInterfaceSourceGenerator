namespace ProxyInterfaceSourceGeneratorTests.Source;

public class Foo2
{
    public Foo2[] Foos { get; set; }

    public Foo2[] DoSomethingAndGetAnArrayOfFoos()
    {
        return new[] { new Foo2() };
    }

    public int Weird { get; set; }

    public int Weird2()
    {
        return 0;
    }
}