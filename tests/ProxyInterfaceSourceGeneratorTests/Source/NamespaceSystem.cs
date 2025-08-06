namespace ProxyInterfaceSourceGeneratorTests.Source.System2;

public class NamespaceSystem
{
    public Task TestAsync1(CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }

    public Task TestAsync2(CancellationToken cancellation)
    {
        return Task.CompletedTask;
    }
}