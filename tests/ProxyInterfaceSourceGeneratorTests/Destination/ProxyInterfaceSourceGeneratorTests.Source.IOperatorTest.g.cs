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
    public partial interface IOperatorTest
    {
        ProxyInterfaceSourceGeneratorTests.Source.OperatorTest _Instance { get; }

        string Name { get; set; }



        ProxyInterfaceSourceGeneratorTests.Source.IOperatorTest op_Implicit(string name);

        string op_Explicit(ProxyInterfaceSourceGeneratorTests.Source.IOperatorTest test);




    }
}
#nullable disable