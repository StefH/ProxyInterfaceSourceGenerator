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
    public partial class ClientRuntimeContextProxy : IClientRuntimeContext
    {
        public Microsoft.SharePoint.Client.ClientRuntimeContext _Instance { get; }
        

        public string Url { get => _Instance.Url; }

        public string ApplicationName { get => _Instance.ApplicationName; set => _Instance.ApplicationName = value; }

        public string ClientTag { get => _Instance.ClientTag; set => _Instance.ClientTag = value; }

        public bool DisableReturnValueCache { get => _Instance.DisableReturnValueCache; set => _Instance.DisableReturnValueCache = value; }

        public bool ValidateOnClient { get => _Instance.ValidateOnClient; set => _Instance.ValidateOnClient = value; }

        public System.Net.ICredentials Credentials { get => _Instance.Credentials; set => _Instance.Credentials = value; }

        public Microsoft.SharePoint.Client.WebRequestExecutorFactory WebRequestExecutorFactory { get => _Instance.WebRequestExecutorFactory; set => _Instance.WebRequestExecutorFactory = value; }

        public Microsoft.SharePoint.Client.ClientRequest PendingRequest { get => _Instance.PendingRequest; }

        public bool HasPendingRequest { get => _Instance.HasPendingRequest; }

        public object Tag { get => _Instance.Tag; set => _Instance.Tag = value; }

        public int RequestTimeout { get => _Instance.RequestTimeout; set => _Instance.RequestTimeout = value; }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public System.Collections.Generic.Dictionary<string, object> StaticObjects { get => _Instance.StaticObjects; }

        public System.Version ServerSchemaVersion { get => _Instance.ServerSchemaVersion; }

        public System.Version ServerLibraryVersion { get => _Instance.ServerLibraryVersion; }

        public System.Version RequestSchemaVersion { get => _Instance.RequestSchemaVersion; set => _Instance.RequestSchemaVersion = value; }

        public string TraceCorrelationId { get => _Instance.TraceCorrelationId; set => _Instance.TraceCorrelationId = value; }



        public virtual void ExecuteQuery()
        {
            _Instance.ExecuteQuery();
        }

        public virtual void RetryQuery(Microsoft.SharePoint.Client.ClientRequest request)
        {
            Microsoft.SharePoint.Client.ClientRequest request_ = request;
            _Instance.RetryQuery(request_);
        }

        public virtual System.Threading.Tasks.Task ExecuteQueryAsync()
        {
            var result_737681611 = _Instance.ExecuteQueryAsync();
            return result_737681611;
        }

        public virtual System.Threading.Tasks.Task RetryQueryAsync(Microsoft.SharePoint.Client.ClientRequest request)
        {
            Microsoft.SharePoint.Client.ClientRequest request_ = request;
            var result_1373930992 = _Instance.RetryQueryAsync(request_);
            return result_1373930992;
        }

        public T CastTo<T>(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject obj) where T : Microsoft.SharePoint.Client.ClientObject
        {
            Microsoft.SharePoint.Client.ClientObject obj_ = Mapster.TypeAdapter.Adapt<Microsoft.SharePoint.Client.ClientObject>(obj);
            var result_366781530 = _Instance.CastTo<T>(obj_);
            return result_366781530;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public void AddQuery(Microsoft.SharePoint.Client.ClientAction query)
        {
            Microsoft.SharePoint.Client.ClientAction query_ = query;
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

        public void AddClientTypeAssembly(System.Reflection.Assembly @assembly)
        {
            System.Reflection.Assembly @assembly_ = @assembly;
            Microsoft.SharePoint.Client.ClientRuntimeContext.AddClientTypeAssembly(@assembly_);
        }

        public void Load<T>(T clientObject, params System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals) where T : Microsoft.SharePoint.Client.ClientObject
        {
            T clientObject_ = clientObject;
            System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals_ = retrievals;
            _Instance.Load<T>(clientObject_, retrievals_);
        }

        public System.Collections.Generic.IEnumerable<T> LoadQuery<T>(Microsoft.SharePoint.Client.ClientObjectCollection<T> clientObjects) where T : Microsoft.SharePoint.Client.ClientObject
        {
            Microsoft.SharePoint.Client.ClientObjectCollection<T> clientObjects_ = clientObjects;
            var result_2035927496 = _Instance.LoadQuery<T>(clientObjects_);
            return result_2035927496;
        }

        public System.Collections.Generic.IEnumerable<T> LoadQuery<T>(System.Linq.IQueryable<T> clientObjects) where T : Microsoft.SharePoint.Client.ClientObject
        {
            System.Linq.IQueryable<T> clientObjects_ = clientObjects;
            var result_2035927496 = _Instance.LoadQuery<T>(clientObjects_);
            return result_2035927496;
        }

        public void Dispose()
        {
            _Instance.Dispose();
        }



        public event System.EventHandler<Microsoft.SharePoint.Client.WebRequestEventArgs> ExecutingWebRequest { add { _Instance.ExecutingWebRequest += value; } remove { _Instance.ExecutingWebRequest -= value; } }





        public ClientRuntimeContextProxy(Microsoft.SharePoint.Client.ClientRuntimeContext instance)
        {
            _Instance = instance;
            

            Mapster.TypeAdapterConfig<Microsoft.SharePoint.Client.ClientRuntimeContext, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext>.NewConfig().ConstructUsing(instance_205293328 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy(instance_205293328));
            Mapster.TypeAdapterConfig<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext, Microsoft.SharePoint.Client.ClientRuntimeContext>.NewConfig().MapWith(proxy1345472640 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientRuntimeContextProxy) proxy1345472640)._Instance);

            Mapster.TypeAdapterConfig<Microsoft.SharePoint.Client.ClientObject, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject>.NewConfig().ConstructUsing(instance_895746668 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy(instance_895746668));
            Mapster.TypeAdapterConfig<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientObject, Microsoft.SharePoint.Client.ClientObject>.NewConfig().MapWith(proxy1674261376 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientObjectProxy) proxy1674261376)._Instance);

            Mapster.TypeAdapterConfig<Microsoft.SharePoint.Client.SecurableObject, ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject>.NewConfig().ConstructUsing(instance592284880 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy(instance592284880));
            Mapster.TypeAdapterConfig<ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject, Microsoft.SharePoint.Client.SecurableObject>.NewConfig().MapWith(proxy_300636294 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.SecurableObjectProxy) proxy_300636294)._Instance);

            Mapster.TypeAdapterConfig<Microsoft.SharePoint.Client.ClientContext, ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext>.NewConfig().ConstructUsing(instance_1283184912 => new ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy(instance_1283184912));
            Mapster.TypeAdapterConfig<ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext, Microsoft.SharePoint.Client.ClientContext>.NewConfig().MapWith(proxy1267236400 => ((ProxyInterfaceSourceGeneratorTests.Source.PnP.ClientContextProxy) proxy1267236400)._Instance);


        }
    }
}
#nullable disable