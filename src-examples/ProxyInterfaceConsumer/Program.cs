using AutoMapper;
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
            IPersonT<int> pT = new PersonTProxy<int>(new PersonT<int>());
            pT.TVal = 1;
            Console.WriteLine(JsonSerializer.Serialize(pT, JsonSerializerOptions));
            Console.WriteLine(new string('-', 80));

            IPersonTT<int, Program> pTT = new PersonTTProxy<int, Program>(new PersonTT<int, Program>());
            pTT.TVal1 = 42;
            pTT.TVal2 = new Program();
            Console.WriteLine(JsonSerializer.Serialize(pTT, JsonSerializerOptions));
            Console.WriteLine(new string('-', 80));

            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            Console.WriteLine("DefaultValue " + p.DefaultValue());
            Console.WriteLine("DefaultValue " + p.DefaultValue(42));

            var ap = new AddressProxy(new Address { HouseNumber = 42 });
            p.Address = ap;
            var add = p.AddAddress(ap);
            Console.WriteLine("add = " + JsonSerializer.Serialize(add, JsonSerializerOptions));
            p.AddAddress(new AddressProxy(new Address { HouseNumber = 1000 }));

            Console.WriteLine(JsonSerializer.Serialize(p, JsonSerializerOptions));
        }
    }
    public struct Test
    {
        public int Id { get; set; }

        public Clazz C { get; }

        public IList<Clazz> Cs { get; set; }

        public int Add(string s)
        {
            return 600;
        }
    }

    public sealed class Clazz
    {
        public string Name { get; set; }
    }

    public interface ITest
    {
        int Id { get; set; }

        IClazz C { get; }

        IList<IClazz> Cs { get; set; }

        int Add(string s);
    }

    public interface IClazz
    {
        string Name { get; set; }
    }

    public class TestProxy : ITest
    {
        private Test _instance;

        private IClazz _clazz;

        private readonly IMapper _mapper;

        public TestProxy(Test instance)
        {
            _instance = instance;

            // _clazz = new ClazzProxy(_instance.C);

            _mapper = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<Clazz, ClazzProxy>();
                //cfg.CreateMap<ClazzProxy, Clazz>();

                cfg.CreateMap<Clazz, IClazz>();
                cfg.CreateMap<IClazz, Clazz>();
            }).CreateMapper();
        }

        public int Id
        {
            get => _instance.Id;
            set => _instance.Id = value;
        }

        public IClazz C => _clazz;

        public IList<IClazz> Cs2
        {
            get
            {
                //return null; // TinyMapper.Map<IList<IClazz>>(_instance.Cs); //(IList<IClazz>)_instance.Cs.Select(x => new ClazzProxy(x));
                return _mapper.Map<IList<IClazz>>(_instance.Cs); //(IList<IClazz>)_instance.Cs.Select(x => new ClazzProxy(x));
            }

            set
            {
                _instance.Cs = _mapper.Map<IList<Clazz>>(value);
                //_instance.Cs = TinyMapper.Map<IList<Clazz>>(value);
            }
        }

        public IList<IClazz> Cs
        {
            get => _mapper.Map<IList<IClazz>>(_instance.Cs);

            set => _instance.Cs = _mapper.Map<IList<Clazz>>(value);
        }

        public int Add(string s) => _instance.Add(s);
    }

    public class ClazzProxy : IClazz
    {
        private Clazz _instance;

        public ClazzProxy(Clazz instance)
        {
            _instance = instance;
        }

        public string Name { get => _instance.Name; set => _instance.Name = value; }
    }
}
