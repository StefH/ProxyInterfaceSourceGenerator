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
    public partial interface ISecurableObject
    {
        new Microsoft.SharePoint.Client.SecurableObject _Instance { get; }

        ProxyInterfaceSourceGeneratorTests.Source.PnP.ISecurableObject FirstUniqueAncestorSecurableObject { get; }

        bool HasUniqueRoleAssignments { get; }

        Microsoft.SharePoint.Client.RoleAssignmentCollection RoleAssignments { get; }



        [Microsoft.SharePoint.Client.RemoteAttribute]
        void ResetRoleInheritance();

        [Microsoft.SharePoint.Client.RemoteAttribute]
        void BreakRoleInheritance(bool copyRoleAssignments, bool clearSubscopes);




    }
}
#nullable disable