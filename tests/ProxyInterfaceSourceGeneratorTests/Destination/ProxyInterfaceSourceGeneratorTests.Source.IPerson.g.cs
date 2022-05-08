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
    public partial interface IPerson
    {
        string Name { get; set; }

        string? StringNullable { get; set; }

        long? NullableLong { get; }

        object @object { get; set; }



        System.Collections.Generic.IList<ProxyInterfaceSourceGeneratorTests.Source.IHuman> AddHuman(ProxyInterfaceSourceGeneratorTests.Source.IHuman h);

        void Void();

        string HelloWorld(string name);

        string HelloWorld2(string? name = "x");

        void WithParams(params string[] values);

        string Add(string s, string @string);

        int DefaultValue(int x = 100);

        void In_Out_Ref1(in int a, out int b, ref int c);

        bool Generic2<T1, T2>(int x, T1 t1, T2 t2) where T1 : struct where T2 : class, new();

        System.Threading.Tasks.Task Method1Async();

        System.Threading.Tasks.Task<int> Method2Async();

        System.Threading.Tasks.Task<string?> Method3Async();

        void CreateInvokeHttpClient(int i = 5, string? appId = null, System.Collections.Generic.IReadOnlyDictionary<string, string> metadata = default, System.Threading.CancellationToken token = default);




    }
}
#nullable disable