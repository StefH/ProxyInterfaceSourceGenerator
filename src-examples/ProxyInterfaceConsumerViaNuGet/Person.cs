using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ProxyInterfaceConsumer
{
    public class Person
    {
        private int PrivateId { get; }
        public int Id { get; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public object @object { get; set; }= null!;

        public long? NullableLong { get; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public string Name { get; set; }= null!;

        public string? StringNullable { get; set; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public Address Address { get; set; }= null!;

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public List<Address> AddressesLIst { get; set; }= null!;

        public Dictionary<string, Address> AddressesDict { get; set; } = new Dictionary<string, Address>();
        public Dictionary<Address, Address> AddressesDict2 { get; set; } = new Dictionary<Address, Address>();

        public E E { get; set; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public IMyInterface MyInterface { get; set; }= null!;

        public string Add(string s, string @string)
        {
            return s + @string;
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

        public int In_Out_Ref2(in IAddress a, out Address b, ref IAddress c)
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
