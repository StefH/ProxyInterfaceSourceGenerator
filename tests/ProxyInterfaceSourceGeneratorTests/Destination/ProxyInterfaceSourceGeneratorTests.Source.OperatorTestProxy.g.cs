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
    public partial class OperatorTestProxy : global::ProxyInterfaceSourceGeneratorTests.Source.IOperatorTest
    {
        public global::ProxyInterfaceSourceGeneratorTests.Source.OperatorTest _Instance { get; }
        
        public string Name { get => _Instance.Name; set => _Instance.Name = value; }

        public int? Id { get => _Instance.Id; set => _Instance.Id = value; }

        public static implicit operator OperatorTestProxy(string name)
        {
            return new OperatorTestProxy((OperatorTest) name);
        }

        public static implicit operator OperatorTestProxy(int? id)
        {
            return new OperatorTestProxy((OperatorTest) id);
        }

        public static explicit operator string(OperatorTestProxy test)
        {
            return (string) test._Instance;
        }

        public static explicit operator int?(OperatorTestProxy test)
        {
            return (int?) test._Instance;
        }


        public OperatorTestProxy(global::ProxyInterfaceSourceGeneratorTests.Source.OperatorTest instance)
        {
            _Instance = instance;
            


        }
    }
}
#nullable restore