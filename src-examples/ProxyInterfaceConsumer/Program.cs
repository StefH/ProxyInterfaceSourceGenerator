using AutoMapper;
using DifferentNamespace;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProxyInterfaceConsumer
{
    public class Program
    {
        private static JsonSerializerOptions JsonSerializerOptions = new ()
        {
            WriteIndented = true
        };

        public static void Main()
        {
            var t = new TestProxy(new Test());

            IPersonT<int> pT = new PersonTProxy<int>(new PersonT<int>());
            pT.TVal = 1;
            Console.WriteLine(JsonSerializer.Serialize(pT, JsonSerializerOptions));
            Console.WriteLine(new string('-', 80));

            //IPersonTT<int, Program> pTT = new PersonTTProxy<int, Program>(new PersonTT<int, Program>());
            //pTT.TVal1 = 42;
            //pTT.TVal2 = new Program();
            //Console.WriteLine(JsonSerializer.Serialize(pTT, JsonSerializerOptions));
            //Console.WriteLine(new string('-', 80));

            var ap = new AddressProxy(new Address { HouseNumber = 42 });
            ap.HouseNumber = -1;
            ap.MyEvent += delegate (object x, EventArgs a)
            {
            };

            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            p.HelloWorld("stef");
            // p.Address = ap;

            Console.WriteLine("DefaultValue " + p.DefaultValue());
            Console.WriteLine("DefaultValue " + p.DefaultValue(42));

            Console.WriteLine(JsonSerializer.Serialize(p, JsonSerializerOptions));
        }
    }

    public class Test
    {
        public int Id { get; set; }

        public Clazz C { get; }

        public IList<Clazz> Cs { get; set; }

        public int AddString(string s)
        {
            return 600;
        }

        public Test AddTest(Test t)
        {
            return new Test();
        }

        public Clazz AddClazz(Clazz c)
        {
            return new Clazz();
        }
    }

    public sealed class Clazz
    {
        public string Name { get; set; }
    }

    [ProxyInterfaceGenerator.Proxy(typeof(Test))]
    public partial interface ITest
    {
    }

    [ProxyInterfaceGenerator.Proxy(typeof(Clazz))]
    public partial interface IClazz
    {
    }
}