using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyClassesGenerator : BaseGenerator, IFilesGenerator
    {
        private readonly List<FileData> files = new List<FileData>();

        public ProxyClassesGenerator(Context context) :
            base(context)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _context.CandidateInterfaces)
            {
                var file = GenerateFile(ci.Value.InterfaceName, ci.Value.ClassName, ci.Value.TypeName, ci.Value.ProxyAll);
                files.Add(file);
            }

            return files;
        }

        private FileData GenerateFile(string interfaceName, string className, string typeName, bool proxyAll)
        {
            var symbol = GetType(typeName);

            var file = new FileData(
                $"{className}Proxy.cs",
                CreateProxyClassCode(symbol, interfaceName, className, proxyAll)
            );

            _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = className, FileData = file });

            return file;
        }

        private string CreateProxyClassCode(INamedTypeSymbol symbol, string interfaceName, string className, bool proxyAll) => $@"using System;
using AutoMapper;

namespace {symbol.ContainingNamespace}
{{
    public class {className}Proxy : {interfaceName}
    {{
        private readonly IMapper _mapper;

        public {className} _Instance {{ get; }}

        public {className}Proxy({className} instance)
        {{
            _Instance = instance;

{GenerateAutoMapper()}
        }}

{GeneratePublicProperties(symbol, proxyAll)}

{GeneratePublicMethods(symbol)}
    }}
}}";

        private string GenerateAutoMapper()
        {
            var str = new StringBuilder();

            str.AppendLine("        _mapper = new MapperConfiguration(cfg =>");
            str.AppendLine("        {");
            foreach (var x in _context.CandidateInterfaces)
            {
                str.AppendLine($"            cfg.CreateMap<{x.Value.InterfaceName}, {x.Value.ClassName}>();");
                str.AppendLine($"            cfg.CreateMap<{x.Value.ClassName}, {x.Value.InterfaceName}>();");
            }
            str.AppendLine("        }).CreateMapper();");

            return str.ToString();
        }

        private string GeneratePublicProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var str = new StringBuilder();

            // SimpleProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.ValueTypeOrString))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // InterfaceProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Interface))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // ComplexProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Complex))
            {
                var type = GetPropertyType(property, out var differs);
                if (!differs.Any())
                {
                    str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                }
                else
                {
                    str.AppendLine($"        public {property.ToPropertyTextForClass(type)}");
                   // var get = property.GetMethod != null ? $"get => _mapper.Map<{type}>(_Instance.{property.Name}); " : string.Empty;
                   // var set = property.SetMethod != null ? $"set => _Instance.{property.Name} = _mapper.Map<{property.Type}>(value);" : string.Empty;
                    //var p = $"{type} {property.Name} {{ {get}{set}}}";
                    //str.AppendLine($"        public {type} {property.Name} {{ {get}{set}}}");
                }

                /*
                 public IList<IClazz> Cs
        {
            get => _mapper.Map<IList<IClazz>>(_instance.Cs);

            set => _instance.Cs = _mapper.Map<IList<Clazz>>(value);
        }
        }*/


                //public static string ToPropertyTextForClass(this IPropertySymbol property, string overrideType)
                //{
                //   // var classNameProxy = $"Proxy";
                //    var get = property.GetMethod != null ? $"get => _mapper.Map<{overrideType}>(_Instance.{property.Name}); " : string.Empty;
                //    var set = property.SetMethod != null ? $"set => _mapper.Map<{property.Type}>( = (({classNameProxy}) value)._Instance; " : string.Empty;

                //    return $"{overrideType} {property.Name} {{ {get}{set}}}";
                //}


                //str.AppendLine($"        public {property.ToPropertyTextForClass(type)}");

                //var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == property.Type.ToString());
                //if (existing is not null)
                //{
                //    str.AppendLine($"        public {property.ToPropertyTextForClass(existing.InterfaceName, existing.ClassName)}");
                //}
                //else
                //{
                //    str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                //}

                str.AppendLine();
            }

            return str.ToString();
        }

        private string GeneratePublicMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        public {method.ToMethodTextForClass()}");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}