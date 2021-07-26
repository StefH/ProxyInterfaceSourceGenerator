using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProxyInterfaceConsumer
{
    public class Person
    {
        private int PrivateId { get; }
        public int Id { get; }

        public long? NullableLong { get; }

        public string Name { get; set; }

        public string? StringNullable { get; set; }

        public Address Address { get; set; }

        public List<Address> AddressesLIst { get; set; }

        public Dictionary<string, Address> AddressesDict { get; set; } = new Dictionary<string, Address>();
        public Dictionary<Address, Address> AddressesDict2 { get; set; } = new Dictionary<Address, Address>();

        public E E { get; set; }

        public IMyInterface MyInterface { get; set; }

        public int Add(string s)
        {
            return 600;
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