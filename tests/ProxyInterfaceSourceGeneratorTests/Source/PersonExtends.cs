using System.Threading.Tasks;

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class PersonExtends : Human
    {
        public string Name { get; set; }

        public string? StringNullable { get; set; }

        public long? NullableLong { get; }

        public object @object { get; set; }

        public void Void()
        {
        }

        public string HelloWorld(string name)
        {
            return $"Hello {name} !";
        }

        public void WithParams(params string[] values)
        {
        }

        public string Add(string s, string @string)
        {
            return s + @string;
        }

        public int DefaultValue(int x = 100)
        {
            return x + 1;
        }

        public void In_Out_Ref1(in int a, out int b, ref int c)
        {
            b = 1;
        }

        public bool Generic2<T1, T2>(int x, T1 t1, T2 t2)
            where T1 : struct
            where T2 : class, new()
        {
            return true;
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
}