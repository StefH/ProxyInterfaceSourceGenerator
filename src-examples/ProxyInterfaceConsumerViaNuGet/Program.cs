using System;
using System.Text.Json;

namespace ProxyInterfaceConsumer
{
    class Program
    {
        private static JsonSerializerOptions JsonSerializerOptions = new ()
        {
            WriteIndented = true
        };

        static void Main(string[] args)
        {
            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            var ap = new AddressProxy(new Address { HouseNumber = 42 });
            p.Address = ap;
            p.AddAddress(ap);
            p.AddAddress(new AddressProxy(new Address { HouseNumber = 1000 }));

            Console.WriteLine(JsonSerializer.Serialize(p, JsonSerializerOptions));
        }
    }
}