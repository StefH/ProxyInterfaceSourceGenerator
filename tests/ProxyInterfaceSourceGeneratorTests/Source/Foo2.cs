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

    public Foo2[] Foos { get; set; }

    public Foo2[] DoSomethingAndGetAnArrayOfFoos()
    {
        return new[] { new Foo2() };
    }

    //public List<Foo> DoSomethingAndGetAListOfFoos()
    //{
    //    return new[] { new Foo() }.ToList();
    //}
}