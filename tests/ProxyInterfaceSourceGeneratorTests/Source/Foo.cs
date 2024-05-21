using System.Diagnostics.CodeAnalysis;

namespace ProxyInterfaceSourceGeneratorTests.Source;

public class Foo
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
    public Foo[] Foos { get; set; } = null!;

    public Foo[] DoSomethingAndGetAnArrayOfFoos()
    {
        return new[] { new Foo() };
    }

    //public List<Foo> DoSomethingAndGetAListOfFoos()
    //{
    //    return new[] { new Foo() }.ToList();
    //}
}
