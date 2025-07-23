using ProxyInterfaceGenerator;

namespace Stef
{
    [Proxy<Class1>)]
    public partial interface IClass1
    {
    }

    [Proxy<Class2>)]
    public partial interface IClass2
    {
    }
}

//namespace Microsoft.Graph.Admin
//{
//    [Proxy(typeof(AdminRequestBuilder), true)]
//    public partial interface IAdminRequestBuilder;
//}

//namespace Microsoft.Graph.Admin.Edge
//{
//    [Proxy(typeof(EdgeRequestBuilder), true)]
//    public partial interface IEdgeRequestBuilder;
//}