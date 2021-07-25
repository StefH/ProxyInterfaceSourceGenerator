using System;

namespace ProxyInterfaceConsumer
{
    public class Program
    {
        public static void Main()
        {
            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            p.Address = new AddressProxy(new Address { HouseNumber = 42 });

            //p.MyNamedTypeSymbol = null;
            //p.Compilation = null;
            //p.Add("x");
            //p.Void();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(p));
        }
    }
}
