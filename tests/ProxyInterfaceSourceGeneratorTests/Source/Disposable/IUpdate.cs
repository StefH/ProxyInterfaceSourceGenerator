namespace ProxyInterfaceSourceGeneratorTests.Source.Disposable
{
    public interface IUpdate<T>
    {
        event EventHandler<T>? Update;

        string Name { get; }
    }
}
