using System.Collections.Generic;

namespace ProxyInterfaceConsumer
{
    public class Person
    {
        private int PrivateId { get; }
        public int Id { get; }

        public long? NullableLong { get; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public List<Address> AddressesLIst { get; set; }

        public Dictionary<string, Address> AddressesDict { get; set; } = new Dictionary<string, Address>();

        public E E { get; set; }

        public IMyInterface MyInterface { get; set; }

        public int Add(string s)
        {
            return 600;
        }

        public void AddAddress(Address a)
        {
            AddressesDict.Add($"{AddressesDict.Count}", a);
        }

        public void Void()
        {
        }
    }

    public enum E
    {
        V1,
        V2
    }
}