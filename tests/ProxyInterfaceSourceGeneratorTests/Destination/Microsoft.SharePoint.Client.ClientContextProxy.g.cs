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

namespace ProxyInterfaceSourceGeneratorTests.Source.PnP
{
    public partial class ClientContextProxy : global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext
    {

        private static global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext MapToInterface(global::Microsoft.SharePoint.Client.ClientRuntimeContext value)
        {
            return new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(value);
        }

        private static global::Microsoft.SharePoint.Client.ClientRuntimeContext MapToInstance(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext value)
        {
            return value._Instance;
        }

        private static global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject MapToInterface(global::Microsoft.SharePoint.Client.ClientObject value)
        {
            return new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(value);
        }

        private static global::Microsoft.SharePoint.Client.ClientObject MapToInstance(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject value)
        {
            return value._Instance;
        }

        private static global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject MapToInterface(global::Microsoft.SharePoint.Client.SecurableObject value)
        {
            return new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy(value);
        }

        private static global::Microsoft.SharePoint.Client.SecurableObject MapToInstance(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject value)
        {
            return value._Instance;
        }

        private static global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext MapToInterface(global::Microsoft.SharePoint.Client.ClientContext value)
        {
            return new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy(value);
        }

        private static global::Microsoft.SharePoint.Client.ClientContext MapToInstance(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext value)
        {
            return value._Instance;
        }

        private static global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb MapToInterface(global::Microsoft.SharePoint.Client.Web value)
        {
            return new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.WebProxy(value);
        }

        private static global::Microsoft.SharePoint.Client.Web MapToInstance(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb value)
        {
            return value._Instance;
        }



        public new global::Microsoft.SharePoint.Client.ClientContext _Instance { get; }
        public global::Microsoft.SharePoint.Client.ClientRuntimeContext _InstanceClientRuntimeContext { get; }
        public global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb Web { get => MapToInterface(_Instance.Web); }

        public global::Microsoft.SharePoint.Client.Site Site { get => _Instance.Site; }

        public global::Microsoft.SharePoint.Client.RequestResources RequestResources { get => _Instance.RequestResources; }

        public global::System.Version ServerVersion { get => _Instance.ServerVersion; }

        public global::Microsoft.SharePoint.Client.FormDigestInfo GetFormDigestDirect()
        {
            var result_333437737 = _Instance.GetFormDigestDirect();
            return result_333437737;
        }

        public override void ExecuteQuery()
        {
            _Instance.ExecuteQuery();
        }

        public override global::System.Threading.Tasks.Task ExecuteQueryAsync()
        {
            var result_737681611 = _Instance.ExecuteQueryAsync();
            return result_737681611;
        }


        public ClientContextProxy(global::Microsoft.SharePoint.Client.ClientContext instance) : base(instance)
        {
            _Instance = instance;
            _InstanceClientRuntimeContext = instance;
        }
    }
}
#nullable restore