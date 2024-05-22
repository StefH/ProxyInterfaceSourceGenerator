using System.Diagnostics.CodeAnalysis;

namespace ProxyInterfaceSourceGeneratorTests.Source;

public class Foo2
{
    //public Bar DoSomethingAndGetABar()
    //{
    //    return new Bar();
    //}

    //public Bar[] DoSomethingAndGetAnArrayOfBars()
    //{
    //    return new[] { new Bar() };
    //}

    //public Foo DoSomethingAndGetAFoo()
    //{
    //    return new Foo();
    //}

    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public Foo2[] Foos { get; set; } = null!;

    public Foo2[] DoSomethingAndGetAnArrayOfFoos()
    {
        return new[] { new Foo2() };
    }

    public int Weird { get; set; }

    public int Weird2()
    {
        return 0;
    }

    //public List<Foo> DoSomethingAndGetAListOfFoos()
    //{
    //    return new[] { new Foo() }.ToList();
    //}
}
