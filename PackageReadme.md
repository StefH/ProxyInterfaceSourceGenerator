# Usage

**Given: an external existing class which does not implement an interface**
``` c#
public sealed class Person
{
    public string Name { get; set; }

    public string HelloWorld(string name)
    {
        return $"Hello {name} !";
    }
}
```

**Create a partial interface**
And annotate this with `ProxyInterfaceGenerator.Proxy[...]` and with the Type which needs to be wrapped:

``` c#
[ProxyInterfaceGenerator.Proxy(typeof(ProxyInterfaceConsumer.Person))]
public partial interface IPerson
{
}
```

When the code is compiled, this source generator creates the following two items:

**1. An additional partial interface**
Which defines the same properties and methods as in the external class.
``` c#
public partial interface IPerson
{
    string Name { get; set; }

    string HelloWorld(string name);
}
```

**2. A Proxy class**
Which takes the external class in the constructor and wraps all properties and methods.

``` c#
public class PersonProxy : IPerson
{
    public Person _Instance { get; }

    public PersonProxy(Person instance)
    {
        _Instance = instance;
    }

    public string Name { get => _Instance.Name; set => _Instance.Name = value; }

    public string HelloWorld(string name)
    {
        string name_ = name;
        var result_19479959 = _Instance.HelloWorld(name_);
        return result_19479959;
    }
}
```

## Use it
``` c#
IPerson p = new PersonProxy(new Person());
p.Name = "test";
p.HelloWorld("stef");
```