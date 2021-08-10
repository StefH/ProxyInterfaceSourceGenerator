using System.Collections.Generic;
using System.Threading.Tasks;
using DifferentNamespace;

namespace ProxyInterfaceConsumer
{
    public class Person
    {
        private int PrivateId { get; }
        public int Id { get; }

        public object @object { get; set; }

        public long? NullableLong { get; }

        public string Name { get; set; }

        public string? StringNullable { get; set; }

        public Address Address { get; set; }

        public List<Address> AddressesList { get; set; }

        public Dictionary<string, Address> AddressesDict { get; set; } = new Dictionary<string, Address>();
        public Dictionary<Address, Address> AddressesDict2 { get; set; } = new Dictionary<Address, Address>();

        public E E { get; set; }

        public IMyInterface MyInterface { get; set; }

        public bool TMethod<T1, T2>(int x, T1 t1, T2 t2)
            where T1 : struct
            where T2 : class, new()
        {
            return true;
        }

        public int DefaultValue(int x = 100)
        {
            return x + 1;
        }

        public string Add(string s, string @string)
        {
            return s + @string;
        }

        public string HelloWorld(string name)
        {
            return $"Hello {name} !";
        }

        public void AddWithParams(params string[] values)
        {
        }

        public Address AddAddress(Address a)
        {
            AddressesDict.Add($"{AddressesDict.Count}", a);

            return a;
        }

        public void AddAddresses(params Address[] addresses)
        {
        }

        public void In_Out_Ref1(in int a, out int b, ref int c)
        {
            b = 1;
        }

        public int In_Out_Ref2(in Address a, out Address b, ref Address c)
        {
            b = new Address();
            return 404;
        }

        public void Void()
        {
        }

        public Task Method1Async()
        {
            return Task.CompletedTask;
        }

        public Task<int> Method2Async()
        {
            return Task.FromResult(1);
        }

        public Task<string?> Method3Async()
        {
            return Task.FromResult((string?)"");
        }
    }

    public enum E
    {
        V1,
        V2
    }
}