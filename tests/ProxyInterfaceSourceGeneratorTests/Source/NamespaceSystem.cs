namespace ProxyInterfaceSourceGeneratorTests.Source.System;

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

    public Task TestAsync3(global::System.Net.Http.HttpClient h)
    {
        return Task.CompletedTask;
    }
}