using Microsoft.CodeAnalysis;

namespace ProxyInterfaceConsumer
{
    public class Person
    {

        private int PrivateId { get; }
        public int Id { get; }

        public long? NullableLong { get; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public E E { get; set; }

        public int Add(string s)
        {
            return 600;
        }

        public void Void()
        {
            //
        }

        public Compilation Compilation { get; set; }

        public INamedTypeSymbol MyNamedTypeSymbol { get;set; }
    }

    

    public enum E
    {
        V1,
        V2
    }
}