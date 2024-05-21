namespace ProxyInterfaceSourceGeneratorTests.Source
{
    /// <summary>
    /// Fun fact, Umlaute are valid in c#
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="KAi"></typeparam>
    public class ÜberGeneric<T1, TKey, KAi>
    {
        public T1 Test(T1 value) => value;

        public KAi Test(KAi value) => value;

        public TKey Test(TKey value) => value;
    }
}
