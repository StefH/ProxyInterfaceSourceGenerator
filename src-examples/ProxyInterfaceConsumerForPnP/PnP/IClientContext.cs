using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientContext))]
    public partial interface IClientContext: IClientRuntimeContext
    {
        // public virtual void X();
    }
}

namespace ProxyInterfaceConsumer.PnP
{
    public partial class ClientContextProxy
    {
        public void Test()
        {
            //var web = _mapper.Map<Web>(Web);
            //Load(web, w => w.Lists);

            Load3(Web, w => w.Lists);
        }

        //public void Load2(IClientObject clientObject, params Expression<Func<IClientObject, object>>[] retrievals)
        //{
        //    ClientObject clientObject_ = _mapper.Map<ClientObject>(clientObject);
        //    Expression<Func<ClientObject, object>>[] retrievals_ = _mapper.Map<Expression<Func<ClientObject, object>>[]>(retrievals);

        //    _Instance.Load(clientObject_, retrievals_);
        //}

        public void Load3(IWeb clientObject, params System.Linq.Expressions.Expression<System.Func<IWeb, object>>[] retrievals)
        {
            var clientObject_ = (WebProxy) clientObject;

            //Expression<Func<WebProxy, object>>[] retrievals_ = _mapper.Map<Expression<Func<WebProxy, object>>[]>(retrievals);

            Load(clientObject_._Instance, null);
        }
    }
}