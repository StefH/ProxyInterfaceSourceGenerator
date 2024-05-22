using System;

namespace ProxyInterfaceConsumer
{
    public class Address
    {
        public int HouseNumber { get; set; }

        public event EventHandler<EventArgs> MyEvent;

        public int Weird { get; set; }

        public int Weird2()
        {
            return 0;
        }
    }
}