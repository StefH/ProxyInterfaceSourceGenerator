using System;
using System.Diagnostics.CodeAnalysis;

namespace ProxyInterfaceConsumer
{
    public class Address
    {
        public int HouseNumber { get; set; }

        [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
        public event EventHandler<EventArgs> MyEvent = null!;
        public int Weird { get; set; }

        public int Weird2()
        {
            return 0;
        }
    }
}
