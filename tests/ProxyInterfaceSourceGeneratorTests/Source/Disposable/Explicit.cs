namespace ProxyInterfaceSourceGeneratorTests.Source.Disposable
{
    public class Explicit : IDisposable, IUpdate<string>
    {
        string IUpdate<string>.Name => throw new NotSupportedException();

        event EventHandler<string>? IUpdate<string>.Update
        {
            add { throw new NotSupportedException(); }
            remove { throw new NotSupportedException(); }
        }

        void IDisposable.Dispose()
        {
            throw new NotSupportedException();
        }
    }
}
