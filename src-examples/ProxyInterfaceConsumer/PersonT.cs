namespace ProxyInterfaceConsumer
{
    public class PersonT<T> where T: class
    {
        public T TVal { get; set; }

        public T Call(int x, T t)
        {
            return default;
        }
    }
}