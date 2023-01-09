//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public partial class PersonExtendsProxy : IPersonExtends
    {
        public ProxyInterfaceSourceGeneratorTests.Source.PersonExtends _Instance { get; }
        

        public string StaticString { get => ProxyInterfaceSourceGeneratorTests.Source.PersonExtends.StaticString; set => ProxyInterfaceSourceGeneratorTests.Source.PersonExtends.StaticString = value; }

        public string Name { get => _Instance.Name; set => _Instance.Name = value; }

        public string? StringNullable { get => _Instance.StringNullable; set => _Instance.StringNullable = value; }

        public long? NullableLong { get => _Instance.NullableLong; }

        public object @object { get => _Instance.@object; set => _Instance.@object = value; }

        public bool IsAlive { get => _Instance.IsAlive; set => _Instance.IsAlive = value; }

        public string GetterOnly { get => _Instance.GetterOnly; }



        public string StaticMethod(int x, string y)
        {
            int x_ = x;
            string y_ = y;
            var result__1647028461 = ProxyInterfaceSourceGeneratorTests.Source.PersonExtends.StaticMethod(x_, y_);
            return result__1647028461;
        }

        public void Void()
        {
            _Instance.Void();
        }

        public string HelloWorld(string name)
        {
            string name_ = name;
            var result_282270798 = _Instance.HelloWorld(name_);
            return result_282270798;
        }

        public void WithParams(params string[] values)
        {
            string[] values_ = values;
            _Instance.WithParams(values_);
        }

        public string Add(string s, string @string)
        {
            string s_ = s;
            string @string_ = @string;
            var result__1127157211 = _Instance.Add(s_, @string_);
            return result__1127157211;
        }

        public int DefaultValue(int x = 100)
        {
            int x_ = x;
            var result__378509684 = _Instance.DefaultValue(x_);
            return result__378509684;
        }

        public void In_Out_Ref1(in int a, out int b, ref int c)
        {
            int a_ = a;
            int b_;
            int c_ = c;
            _Instance.In_Out_Ref1(in a_, out b_, ref c_);
            b = b_;
        }

        public bool Generic2<T1, T2>(int x, T1 t1, T2 t2) where T1 : struct where T2 : class, new()
        {
            int x_ = x;
            T1 t1_ = t1;
            T2 t2_ = t2;
            var result_542538942 = _Instance.Generic2<T1, T2>(x_, t1_, t2_);
            return result_542538942;
        }

        public System.Threading.Tasks.Task Method1Async()
        {
            var result__57678382 = _Instance.Method1Async();
            return result__57678382;
        }

        public System.Threading.Tasks.Task<int> Method2Async()
        {
            var result__57677169 = _Instance.Method2Async();
            return result__57677169;
        }

        public System.Threading.Tasks.Task<string?> Method3Async()
        {
            var result__57684656 = _Instance.Method3Async();
            return result__57684656;
        }







        public PersonExtendsProxy(ProxyInterfaceSourceGeneratorTests.Source.PersonExtends instance)
        {
            _Instance = instance;
            


        }
    }
}
#nullable disable