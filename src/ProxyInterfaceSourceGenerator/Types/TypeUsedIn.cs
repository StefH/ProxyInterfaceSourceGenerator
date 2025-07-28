namespace ProxyInterfaceSourceGenerator.Types;

[Flags]
internal enum TypeUsedIn : byte
{
    None = 0,

    // MapToInterface
    Get = 1,

    // MapToInstance
    Set = 2,

    Indexer = 4,

    // MapToInterface and MapToInstance
    Method = 8,

    Event = 16,

    Operator = 32,

    // MapToInstance
    Constraint = 64,

    MapToInterface = Get | Method,

    MapToInstance = Set | Method
}