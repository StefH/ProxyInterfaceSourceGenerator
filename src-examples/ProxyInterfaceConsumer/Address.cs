using System;

namespace ProxyInterfaceConsumer
{
    public class Address
    {
        public int HouseNumber { get; set; }

        public event EventHandler<EventArgs> MyEvent;
    }
}