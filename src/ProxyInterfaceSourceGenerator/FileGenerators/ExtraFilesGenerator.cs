using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal class ExtraFilesGenerator : IFileGenerator
{
    private const string Name = "ProxyInterfaceGenerator.Extra.g.cs";

    public FileData GenerateFile(bool supportsNullable, bool supportsGenericAttributes)
    {
        var stringArray = supportsNullable ? "string[]?" : "string[]";

        var attribute = $@"
    [AttributeUsage(AttributeTargets.Interface)]
    internal sealed class ProxyAttribute : Attribute
    {{
        public Type Type {{ get; }}
        public bool ProxyBaseClasses {{ get; }}
        public ProxyClassAccessibility Accessibility {{ get; }}
        public {stringArray} MembersToIgnore {{ get; }}

        public ProxyAttribute(Type type) : this(type, false, ProxyClassAccessibility.Public)
        {{
        }}

        public ProxyAttribute(Type type, bool proxyBaseClasses) : this(type, proxyBaseClasses, ProxyClassAccessibility.Public)
        {{
        }}

        public ProxyAttribute(Type type, ProxyClassAccessibility accessibility) : this(type, false, accessibility)
        {{
        }}

        public ProxyAttribute(Type type, ProxyClassAccessibility accessibility, {stringArray} membersToIgnore) : this(type, false, accessibility, membersToIgnore)
        {{
        }}

        public ProxyAttribute(Type type, bool proxyBaseClasses, ProxyClassAccessibility accessibility) : this(type, proxyBaseClasses, accessibility, null)
        {{
        }}

        public ProxyAttribute(Type type, {stringArray} membersToIgnore) : this(type, false, ProxyClassAccessibility.Public, null)
        {{
        }}

        public ProxyAttribute(Type type, bool proxyBaseClasses, ProxyClassAccessibility accessibility, {stringArray} membersToIgnore)
        {{
            Type = type;
            ProxyBaseClasses = proxyBaseClasses;
            Accessibility = accessibility;
            MembersToIgnore = membersToIgnore;
        }}
    }}";

        var genericAttribute = $@"
    [AttributeUsage(AttributeTargets.Interface)]
    internal sealed class ProxyAttribute<T> : Attribute where T : class
    {{
        public bool ProxyBaseClasses {{ get; }}
        public ProxyClassAccessibility Accessibility {{ get; }}
        public {stringArray} MembersToIgnore {{ get; }}

        public ProxyAttribute() : this(false, ProxyClassAccessibility.Public)
        {{
        }}

        public ProxyAttribute(bool proxyBaseClasses) : this(type, proxyBaseClasses, ProxyClassAccessibility.Public)
        {{
        }}

        public ProxyAttribute(ProxyClassAccessibility accessibility) : this(type, false, accessibility)
        {{
        }}

        public ProxyAttribute(ProxyClassAccessibility accessibility, {stringArray} membersToIgnore) : this(type, false, accessibility, membersToIgnore)
        {{
        }}

        public ProxyAttribute(bool proxyBaseClasses, ProxyClassAccessibility accessibility) : this(type, proxyBaseClasses, accessibility, null)
        {{
        }}

        public ProxyAttribute({stringArray} membersToIgnore) : this(type, false, ProxyClassAccessibility.Public, null)
        {{
        }}

        public ProxyAttribute(bool proxyBaseClasses, ProxyClassAccessibility accessibility, {stringArray} membersToIgnore)
        {{
            Type = typeof(T);
            ProxyBaseClasses = proxyBaseClasses;
            Accessibility = accessibility;
            MembersToIgnore = membersToIgnore;
        }}
    }}";

        return new FileData($"{Name}", $@"//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

using System;

namespace ProxyInterfaceGenerator
{{
    {attribute}

    {supportsGenericAttributes.IIf(genericAttribute)}

    [Flags]
    internal enum ProxyClassAccessibility
    {{
        Public = 0,

        Internal = 1
    }}
}}");
    }
}