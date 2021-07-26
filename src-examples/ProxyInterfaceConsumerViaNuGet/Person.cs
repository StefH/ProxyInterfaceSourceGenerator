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

        public Address AddAddress(Address a)
        {
            AddressesDict.Add($"{AddressesDict.Count}", a);

            return a;
        }

        public void In_Out_Ref1(in int a, out int b, ref int c)
        {
            b = 1;
        }

        public void In_Out_Ref2(in Address a, out Address b, ref Address c)
        {
            b = new Address();
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