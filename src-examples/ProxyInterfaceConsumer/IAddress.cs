using DifferentNamespace;
using System;
using System.Linq;

namespace ProxyInterfaceConsumer
{
    [ProxyInterfaceGenerator.Proxy(typeof(Address))]
    public partial interface IAddress
    {
    }
}