namespace ProxyInterfaceConsumer
{
    public class PersonT<T> where T: struct
    {
        public T TVal { get; set; }

        public T Call(int x, T t)
        {
            return default;
        }
    }
}