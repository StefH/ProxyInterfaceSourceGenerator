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
    public partial class SecurableObjectProxy : global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject
    {
        static SecurableObjectProxy()
        {
            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientRuntimeContext, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>.NewConfig().ConstructUsing(instance_572349648 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(instance_572349648));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext, global::Microsoft.SharePoint.Client.ClientRuntimeContext>.NewConfig().MapWith(proxy214349770 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy) proxy214349770)._Instance);
        }
        static SecurableObjectProxy()
        {
            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientObject, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>.NewConfig().ConstructUsing(instance_205438316 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(instance_205438316));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject, global::Microsoft.SharePoint.Client.ClientObject>.NewConfig().MapWith(proxy_437526006 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy) proxy_437526006)._Instance);
        }
        static SecurableObjectProxy()
        {
            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.SecurableObject, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject>.NewConfig().ConstructUsing(instance_247129254 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy(instance_247129254));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject, global::Microsoft.SharePoint.Client.SecurableObject>.NewConfig().MapWith(proxy_117192422 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy) proxy_117192422)._Instance);
        }


        public new global::Microsoft.SharePoint.Client.SecurableObject _Instance { get; }
        public global::Microsoft.SharePoint.Client.ClientObject _InstanceClientObject { get; }
        [Microsoft.SharePoint.Client.RemoteAttribute]
        public global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject FirstUniqueAncestorSecurableObject { get => Mapster.TypeAdapter.Adapt<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject>(_Instance.FirstUniqueAncestorSecurableObject); }

        [Microsoft.SharePoint.Client.RemoteAttribute]
        public bool HasUniqueRoleAssignments { get => _Instance.HasUniqueRoleAssignments; }

        [Microsoft.SharePoint.Client.RemoteAttribute]
        public global::Microsoft.SharePoint.Client.RoleAssignmentCollection RoleAssignments { get => _Instance.RoleAssignments; }

        [Microsoft.SharePoint.Client.RemoteAttribute]
        public virtual void ResetRoleInheritance()
        {
            _Instance.ResetRoleInheritance();
        }

        [Microsoft.SharePoint.Client.RemoteAttribute]
        public virtual void BreakRoleInheritance(bool copyRoleAssignments, bool clearSubscopes)
        {
            bool copyRoleAssignments_ = copyRoleAssignments;
            bool clearSubscopes_ = clearSubscopes;
            _Instance.BreakRoleInheritance(copyRoleAssignments_, clearSubscopes_);
        }


        public SecurableObjectProxy(global::Microsoft.SharePoint.Client.SecurableObject instance) : base(instance)
        {
            _Instance = instance;
            _InstanceClientObject = instance;
        }
    }
}
#nullable restore