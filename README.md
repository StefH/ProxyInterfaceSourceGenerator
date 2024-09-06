# ProxyInterfaceGenerator
This project uses Source Generation to generate an interface and a Proxy class for classes.
This makes it possible to wrap external classes which do not have an interface, in a Proxy class which makes it easier to Mock and use DI.

It supports:
- properties
- methods
- events
- implicit and explicit operators

## Install
[![NuGet Badge](https://img.shields.io/nuget/v/ProxyInterfaceGenerator)](https://www.nuget.org/packages/ProxyInterfaceGenerator)

You can install from NuGet using the following command in the package manager window:

`Install-Package ProxyInterfaceGenerator`

Or via the Visual Studio NuGet package manager or if you use the `dotnet` command:

`dotnet add package ProxyInterfaceGenerator`

## Usage
### Given: an external existing class which does not implement an interface
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

### Create a partial interface
And annotate this with `ProxyInterfaceGenerator.Proxy[...]` and with the Type which needs to be wrapped:

``` c#
[ProxyInterfaceGenerator.Proxy(typeof(Person))]
public partial interface IPerson
{
}
```

#### ProxyBaseClasses
In case also want to proxy the properties/methods/events from the base class(es), use this:

``` c#
[ProxyInterfaceGenerator.Proxy(typeof(Person), true)] // 👈 Provide `true` as second parameter.
public partial interface IPerson
{
}
```

#### ProxyClassAccessibility
By default, the generated Proxy class is `public`. If you want to create the Proxy class as `internal`, use the following:

``` c#
[ProxyInterfaceGenerator.Proxy(typeof(Person), ProxyClassAccessibility.Internal)] // 👈 Provide `ProxyClassAccessibility.Internal` as second parameter.
public partial interface IPerson
{
}
```

### When the code is compiled, this source generator creates the following

#### :one: An additional partial interface
Which defines the same properties and methods as in the external class.
``` c#
public partial interface IPerson
{
    string Name { get; set; }

    string HelloWorld(string name);
}
```

#### :two: A Proxy class
Which takes the external class in the constructor and wraps all public properties, events and methods.

``` c#
// ⭐
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

:star: By default the accessibility from the generated Proxy class is `public`.

### :three: Use it
``` c#
IPerson p = new PersonProxy(new Person());
p.Name = "test";
p.HelloWorld("stef");
```

# References
- https://route2roslyn.netlify.app/symbols-for-dummies/
