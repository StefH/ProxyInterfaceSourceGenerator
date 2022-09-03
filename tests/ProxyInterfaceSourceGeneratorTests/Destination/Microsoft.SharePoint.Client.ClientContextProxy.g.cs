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
using AutoMapper;

namespace ProxyInterfaceSourceGeneratorTests.Source.PnP
{
    public partial class ClientContextProxy : ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy, IClientContext
    {
        public new Microsoft.SharePoint.Client.ClientContext _Instance { get; }
        public Microsoft.SharePoint.Client.ClientRuntimeContext _InstanceBase { get; }


        public ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb Web { get => _mapper.Map<ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb>(_Instance.Web); }

        public Microsoft.SharePoint.Client.Site Site { get => _Instance.Site; }

        public Microsoft.SharePoint.Client.RequestResources RequestResources { get => _Instance.RequestResources; }

        public System.Version ServerVersion { get => _Instance.ServerVersion; }



        public Microsoft.SharePoint.Client.FormDigestInfo GetFormDigestDirect()
        {
            var result_333437737 = _Instance.GetFormDigestDirect();
            return result_333437737;
        }

        public override void ExecuteQuery()
        {
            _Instance.ExecuteQuery();
        }

        public override System.Threading.Tasks.Task ExecuteQueryAsync()
        {
            var result_737681611 = _Instance.ExecuteQueryAsync();
            return result_737681611;
        }





        public ClientContextProxy(Microsoft.SharePoint.Client.ClientContext instance) : base(instance)
        {
            _Instance = instance;
            _InstanceBase = instance;

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Microsoft.SharePoint.Client.ClientRuntimeContext, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>().ConstructUsing(instance_205293328 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(instance_205293328));
                cfg.CreateMap<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext, Microsoft.SharePoint.Client.ClientRuntimeContext>().ConstructUsing(proxy1345472640 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy) proxy1345472640)._Instance);
                cfg.CreateMap<Microsoft.SharePoint.Client.ClientObject, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>().ConstructUsing(instance_895746668 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(instance_895746668));
                cfg.CreateMap<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject, Microsoft.SharePoint.Client.ClientObject>().ConstructUsing(proxy1674261376 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy) proxy1674261376)._Instance);
                cfg.CreateMap<Microsoft.SharePoint.Client.SecurableObject, ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject>().ConstructUsing(instance592284880 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy(instance592284880));
                cfg.CreateMap<ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject, Microsoft.SharePoint.Client.SecurableObject>().ConstructUsing(proxy_300636294 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy) proxy_300636294)._Instance);
                cfg.CreateMap<Microsoft.SharePoint.Client.ClientContext, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext>().ConstructUsing(instance_1283184912 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy(instance_1283184912));
                cfg.CreateMap<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext, Microsoft.SharePoint.Client.ClientContext>().ConstructUsing(proxy1267236400 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy) proxy1267236400)._Instance);
                cfg.CreateMap<Microsoft.SharePoint.Client.Web, ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb>().ConstructUsing(instance_1865313808 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.WebProxy(instance_1865313808));
                cfg.CreateMap<ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb, Microsoft.SharePoint.Client.Web>().ConstructUsing(proxy2115366516 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.WebProxy) proxy2115366516)._Instance);
            }).CreateMapper();

        }

        private readonly IMapper _mapper;
    }
}
#nullable disable