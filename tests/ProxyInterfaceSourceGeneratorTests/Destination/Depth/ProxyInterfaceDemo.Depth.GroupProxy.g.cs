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

namespace ProxyInterfaceDemo.Depth
{
    public partial class GroupProxy : global::ProxyInterfaceDemo.Depth.DestroyableProxy, global::ProxyInterfaceDemo.Depth.IGroup
    {


        public new global::ProxyInterfaceDemo.Depth.Group _Instance { get; }
        public global::ProxyInterfaceDemo.Depth.Destroyable _InstanceDestroyable { get; }

        public GroupProxy(global::ProxyInterfaceDemo.Depth.Group instance) : base(instance)
        {
            _Instance = instance;
            _InstanceDestroyable = instance;
        }
    }
}
#nullable restore