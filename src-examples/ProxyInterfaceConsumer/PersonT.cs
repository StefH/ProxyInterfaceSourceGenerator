namespace ProxyInterfaceConsumer
{
    public class PersonT<T>
    {
        public T TVal { get; set; }

        public T Call(int x, T t)
        {
            return default;
        }
    }
}