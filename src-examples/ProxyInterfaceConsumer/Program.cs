﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Microsoft.CodeAnalysis;

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
            var c = new Clazz
            {
                Name = "n"
            };
            var cp = new ClazzProxy(c);

            var t = new Test();
            t.Cs = new List<Clazz> { c };

            var tp = new TestProxy(t);
            tp.Cs = new List<IClazz> { cp };

            Console.WriteLine(JsonSerializer.Serialize(t, JsonSerializerOptions));
            Console.WriteLine(new string('-', 80));


            IPerson p = new PersonProxy(new Person());
            p.Name = "test";
            var ap = new AddressProxy(new Address { HouseNumber = 42 });
            p.Address = ap;
            var add = p.AddAddress(ap);
            Console.WriteLine("add = " + JsonSerializer.Serialize(add, JsonSerializerOptions));
            p.AddAddress(new AddressProxy(new Address { HouseNumber = 1000 }));

            //p.MyNamedTypeSymbol = null;
            //p.Compilation = null;
            //p.Add("x");
            //p.Void();
            Console.WriteLine(JsonSerializer.Serialize(p, JsonSerializerOptions));

            //GeneratorExecutionContext g = new GeneratorExecutionContext();
            //IGeneratorExecutionContext gc = new GeneratorExecutionContextProxy(g);
            //int y = 9;
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
