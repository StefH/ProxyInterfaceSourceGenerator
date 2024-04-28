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
    public partial class ClientObjectProxy : global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject
    {
        public global::Microsoft.SharePoint.Client.ClientObject _Instance { get; }
        
        public global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext Context { get => Mapster.TypeAdapter.Adapt<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>(_Instance.Context); }

        public object Tag { get => _Instance.Tag; set => _Instance.Tag = value; }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public global::Microsoft.SharePoint.Client.ObjectPath Path { get => _Instance.Path; }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public string ObjectVersion { get => _Instance.ObjectVersion; set => _Instance.ObjectVersion = value; }

        [Microsoft.SharePoint.Client.PseudoRemoteAttribute]
        public bool? ServerObjectIsNull { get => _Instance.ServerObjectIsNull; }

        public global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject TypedObject { get => Mapster.TypeAdapter.Adapt<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>(_Instance.TypedObject); }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void FromJson(global::Microsoft.SharePoint.Client.JsonReader reader)
        {
            global::Microsoft.SharePoint.Client.JsonReader reader_ = reader;
            _Instance.FromJson(reader_);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual bool CustomFromJson(global::Microsoft.SharePoint.Client.JsonReader reader)
        {
            global::Microsoft.SharePoint.Client.JsonReader reader_ = reader;
            var result__636829107 = _Instance.CustomFromJson(reader_);
            return result__636829107;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void Retrieve()
        {
            _Instance.Retrieve();
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void Retrieve(params string[] propertyNames)
        {
            string[] propertyNames_ = propertyNames;
            _Instance.Retrieve(propertyNames_);
        }

        public virtual void RefreshLoad()
        {
            _Instance.RefreshLoad();
        }

        public bool IsPropertyAvailable(string propertyName)
        {
            string propertyName_ = propertyName;
            var result_1607091274 = _Instance.IsPropertyAvailable(propertyName_);
            return result_1607091274;
        }

        public bool IsObjectPropertyInstantiated(string propertyName)
        {
            string propertyName_ = propertyName;
            var result__181021484 = _Instance.IsObjectPropertyInstantiated(propertyName_);
            return result__181021484;
        }


        public ClientObjectProxy(global::Microsoft.SharePoint.Client.ClientObject instance)
        {
            _Instance = instance;
            

            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientRuntimeContext, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>.NewConfig().ConstructUsing(instance_572349648 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(instance_572349648));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext, global::Microsoft.SharePoint.Client.ClientRuntimeContext>.NewConfig().MapWith(proxy214349770 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy) proxy214349770)._Instance);

            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientObject, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>.NewConfig().ConstructUsing(instance_205438316 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(instance_205438316));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject, global::Microsoft.SharePoint.Client.ClientObject>.NewConfig().MapWith(proxy_437526006 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy) proxy_437526006)._Instance);


        }
    }
}
#nullable restore