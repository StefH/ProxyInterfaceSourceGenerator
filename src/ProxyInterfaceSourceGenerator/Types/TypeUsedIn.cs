namespace ProxyInterfaceSourceGenerator.Types;

[Flags]
internal enum TypeUsedIn
{
    None = 0,

    // MapToInterface
    Get = 1,

    // MapToInstance
    Set = 2,

    GetAndSet = Get | Set,

    Indexer = 4,

    // MapToInterface and MapToInstance
    Method = 8,

    Event = 16,

    Operator = 32,

    Constraint = 64,

    MapToInterface = Get | Method,

    MapToInstance = Set | Method
}