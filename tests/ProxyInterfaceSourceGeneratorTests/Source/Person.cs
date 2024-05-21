using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class Person : Human
    {
        private readonly MyStruct[] _arr = new MyStruct[1];

        [Display(Prompt = "MyStruct Indexer")]
        public MyStruct this[int i]
        {
            get { return _arr[i]; }
            set { _arr[i] = value; }
        }

        public MyStruct this[int i, string s]
        {
            get { return _arr[i]; }
            set { _arr[i] = value; }
        }

        public IList<Human> AddHuman(Human h)
        {
            return new List<Human> { h, new Human { IsAlive = true } };
        }

        [Display(ResourceType = typeof(PeriodicTimer))]
        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public string Name { get; set; }= null!;

        public string? StringNullable { get; set; }

        public long? NullableLong { get; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public object @object { get; set; }= null!;

        public void Void()
        {
        }

        public string HelloWorld(string name)
        {
            return $"Hello {name} !";
        }

        public string HelloWorld2(string? name = "x")
        {
            return $"Hello {name} !";
        }

        public string HelloWorld3(char? ch = 'c')
        {
            return $"Hello {ch} !";
        }

        public string HelloWorld4(char ch)
        {
            return $"Hello {ch} !";
        }

        public string HelloWorld5(char? ch)
        {
            return $"Hello {ch} !";
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
            c++;
        }

        public double[,] Out_MultiDimensionIssue54(out double[,] x)
        {
            x = new double[0, 0];
            return x;
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

        [Display(Name = "M3")]
        public Task<string?> Method3Async()
        {
            return Task.FromResult((string?)"");
        }

        public void CreateInvokeHttpClient(int i = 5, string? appId = null, IReadOnlyDictionary<string, string>? metadata = null, CancellationToken token = default)
        {
        }

        public bool TryParse(string s1, [NotNullWhen(true)]params int[]? ii)
        {
            ii = null;
            return false;
        }

        public bool TryParse(string s2, [NotNullWhen(true)] out int? i)
        {
            i = 4;
            return true;
        }
    }
}
