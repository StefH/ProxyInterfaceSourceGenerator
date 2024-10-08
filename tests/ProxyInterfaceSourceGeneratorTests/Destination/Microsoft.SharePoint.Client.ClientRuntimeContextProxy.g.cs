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
    public partial class ClientRuntimeContextProxy : global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext
    {
        static ClientRuntimeContextProxy()
        {
            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientRuntimeContext, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>.NewConfig().ConstructUsing(instance_572349648 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(instance_572349648));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext, global::Microsoft.SharePoint.Client.ClientRuntimeContext>.NewConfig().MapWith(proxy214349770 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy) proxy214349770)._Instance);

            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientObject, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>.NewConfig().ConstructUsing(instance_205438316 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(instance_205438316));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject, global::Microsoft.SharePoint.Client.ClientObject>.NewConfig().MapWith(proxy_437526006 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy) proxy_437526006)._Instance);

            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.SecurableObject, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject>.NewConfig().ConstructUsing(instance_247129254 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy(instance_247129254));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject, global::Microsoft.SharePoint.Client.SecurableObject>.NewConfig().MapWith(proxy_117192422 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy) proxy_117192422)._Instance);

            Mapster.TypeAdapterConfig<global::Microsoft.SharePoint.Client.ClientContext, global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext>.NewConfig().ConstructUsing(instance_1483513702 => new global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy(instance_1483513702));
            Mapster.TypeAdapterConfig<global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext, global::Microsoft.SharePoint.Client.ClientContext>.NewConfig().MapWith(proxy343311664 => ((global::ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy) proxy343311664)._Instance);

        }


        public global::Microsoft.SharePoint.Client.ClientRuntimeContext _Instance { get; }
        
        public event global::System.EventHandler<global::Microsoft.SharePoint.Client.WebRequestEventArgs> ExecutingWebRequest { add { _Instance.ExecutingWebRequest += value; } remove { _Instance.ExecutingWebRequest -= value; } }

        public string Url { get => _Instance.Url; }

        public string ApplicationName { get => _Instance.ApplicationName; set => _Instance.ApplicationName = value; }

        public string ClientTag { get => _Instance.ClientTag; set => _Instance.ClientTag = value; }

        public bool DisableReturnValueCache { get => _Instance.DisableReturnValueCache; set => _Instance.DisableReturnValueCache = value; }

        public bool ValidateOnClient { get => _Instance.ValidateOnClient; set => _Instance.ValidateOnClient = value; }

        public global::System.Net.ICredentials Credentials { get => _Instance.Credentials; set => _Instance.Credentials = value; }

        public global::Microsoft.SharePoint.Client.WebRequestExecutorFactory WebRequestExecutorFactory { get => _Instance.WebRequestExecutorFactory; set => _Instance.WebRequestExecutorFactory = value; }

        public global::Microsoft.SharePoint.Client.ClientRequest PendingRequest { get => _Instance.PendingRequest; }

        public bool HasPendingRequest { get => _Instance.HasPendingRequest; }

        public object Tag { get => _Instance.Tag; set => _Instance.Tag = value; }

        public int RequestTimeout { get => _Instance.RequestTimeout; set => _Instance.RequestTimeout = value; }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public global::System.Collections.Generic.Dictionary<string, object> StaticObjects { get => _Instance.StaticObjects; }

        public global::System.Version ServerSchemaVersion { get => _Instance.ServerSchemaVersion; }

        public global::System.Version ServerLibraryVersion { get => _Instance.ServerLibraryVersion; }

        public global::System.Version RequestSchemaVersion { get => _Instance.RequestSchemaVersion; set => _Instance.RequestSchemaVersion = value; }

        public string TraceCorrelationId { get => _Instance.TraceCorrelationId; set => _Instance.TraceCorrelationId = value; }

        public virtual void ExecuteQuery()
        {
            _Instance.ExecuteQuery();
        }

        public virtual void RetryQuery(global::Microsoft.SharePoint.Client.ClientRequest request)
        {
            global::Microsoft.SharePoint.Client.ClientRequest request_ = request;
            _Instance.RetryQuery(request_);
        }

        public virtual global::System.Threading.Tasks.Task ExecuteQueryAsync()
        {
            var result_737681611 = _Instance.ExecuteQueryAsync();
            return result_737681611;
        }

        public virtual global::System.Threading.Tasks.Task RetryQueryAsync(global::Microsoft.SharePoint.Client.ClientRequest request)
        {
            global::Microsoft.SharePoint.Client.ClientRequest request_ = request;
            var result_1373930992 = _Instance.RetryQueryAsync(request_);
            return result_1373930992;
        }

        public T CastTo<T>(global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject obj) where T : Microsoft.SharePoint.Client.ClientObject
        {
            global::Microsoft.SharePoint.Client.ClientObject obj_ = Mapster.TypeAdapter.Adapt<global::Microsoft.SharePoint.Client.ClientObject>(obj);
            var result_366781530 = _Instance.CastTo<T>(obj_);
            return result_366781530;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void AddQuery(global::Microsoft.SharePoint.Client.ClientAction query)
        {
            global::Microsoft.SharePoint.Client.ClientAction query_ = query;
            _Instance.AddQuery(query_);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void AddQueryIdAndResultObject(long id, object obj)
        {
            long id_ = id;
            object obj_ = obj;
            _Instance.AddQueryIdAndResultObject(id_, obj_);
        }

        public object ParseObjectFromJsonString(string json)
        {
            string json_ = json;
            var result__1648501661 = _Instance.ParseObjectFromJsonString(json_);
            return result__1648501661;
        }

        public void AddClientTypeAssembly(global::System.Reflection.Assembly @assembly)
        {
            global::System.Reflection.Assembly @assembly_ = @assembly;
            global::Microsoft.SharePoint.Client.ClientRuntimeContext.AddClientTypeAssembly(@assembly_);
        }

        public void Load<T>(T clientObject, params global::System.Linq.Expressions.Expression<global::System.Func<T, object>>[] retrievals) where T : Microsoft.SharePoint.Client.ClientObject
        {
            T clientObject_ = clientObject;
            global::System.Linq.Expressions.Expression<global::System.Func<T, object>>[] retrievals_ = retrievals;
            _Instance.Load<T>(clientObject_, retrievals_);
        }

        public global::System.Collections.Generic.IEnumerable<T> LoadQuery<T>(global::Microsoft.SharePoint.Client.ClientObjectCollection<T> clientObjects) where T : Microsoft.SharePoint.Client.ClientObject
        {
            global::Microsoft.SharePoint.Client.ClientObjectCollection<T> clientObjects_ = clientObjects;
            var result_2035927496 = _Instance.LoadQuery<T>(clientObjects_);
            return result_2035927496;
        }

        public global::System.Collections.Generic.IEnumerable<T> LoadQuery<T>(global::System.Linq.IQueryable<T> clientObjects) where T : Microsoft.SharePoint.Client.ClientObject
        {
            global::System.Linq.IQueryable<T> clientObjects_ = clientObjects;
            var result_2035927496 = _Instance.LoadQuery<T>(clientObjects_);
            return result_2035927496;
        }

        public void Dispose()
        {
            _Instance.Dispose();
        }


        public ClientRuntimeContextProxy(global::Microsoft.SharePoint.Client.ClientRuntimeContext instance)
        {
            _Instance = instance;
            
        }
    }
}
#nullable restore