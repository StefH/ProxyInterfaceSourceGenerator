using System;

namespace DifferentNamespace
{
    public class Address
    {
        public int HouseNumber { get; set; }

        public event EventHandler<EventArgs> MyEvent;
    }
}