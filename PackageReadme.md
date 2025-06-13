## ProxyInterfaceGenerator
This project uses Source Generation to generate an interface and a Proxy class for classes.
This makes it possible to wrap external classes which do not have an interface, in a Proxy class which makes it easier to Mock and use DI.

It supports:
- properties
- methods
- events
- implicit and explicit operators


### Info

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

### Use it
``` c#
IPerson p = new PersonProxy(new Person());
p.Name = "test";
p.HelloWorld("stef");
```


### Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of **ProxyInterfaceSourceGenerator**.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)