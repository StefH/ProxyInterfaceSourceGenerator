namespace ProxyInterfaceConsumer
{
    public class PersonTT<T1, T2>
        where T1 : struct
        where T2 : class, new()
    {
        public T1 TVal1 { get; set; }

        public T2 TVal2 { get; set; }

        public void Call(int x, T1 t1, T2 t2)
        {
        }
    }
}