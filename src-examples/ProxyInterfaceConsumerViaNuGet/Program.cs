using System;
using System.Text.Json;

namespace ProxyInterfaceConsumer
{
    class Program
    {
        private static JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

        static void Main(string[] args)
        {
            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            var ap = new AddressProxy(new Address { HouseNumber = 42 });
            p.Address = ap;
            var add = p.AddAddress(ap);
            Console.WriteLine("add = " + JsonSerializer.Serialize(add, JsonSerializerOptions));

            p.AddAddress(new AddressProxy(new Address { HouseNumber = 1000 }));

            Console.WriteLine(JsonSerializer.Serialize(p, JsonSerializerOptions));
        }
    }
}
