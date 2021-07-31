using System;
using AutoMapper;

namespace ProxyInterfaceConsumer
{
    public class PersonProxy2 : IPerson
    {
        public ProxyInterfaceConsumer.Person _Instance { get; }

        public int Id { get => _Instance.Id; }

        public object @object { get => _Instance.@object; set => _Instance.@object = value; }

    public long? NullableLong { get => _Instance.NullableLong; }

    public string Name { get => _Instance.Name; set => _Instance.Name = value; }

    public string? StringNullable { get => _Instance.StringNullable; set => _Instance.StringNullable = value; }

    public IAddress Address { get => _mapper.Map<IAddress>(_Instance.Address); set => _Instance.Address = _mapper.Map<ProxyInterfaceConsumer.Address>(value); }

    public System.Collections.Generic.List<IAddress> AddressesLIst { get => _mapper.Map<System.Collections.Generic.List<IAddress>>(_Instance.AddressesLIst); set => _Instance.AddressesLIst = _mapper.Map<System.Collections.Generic.List<ProxyInterfaceConsumer.Address>>(value); }

    public System.Collections.Generic.Dictionary<string, IAddress> AddressesDict { get => _mapper.Map<System.Collections.Generic.Dictionary<string, IAddress>>(_Instance.AddressesDict); set => _Instance.AddressesDict = _mapper.Map<System.Collections.Generic.Dictionary<string, ProxyInterfaceConsumer.Address>>(value); }

    public System.Collections.Generic.Dictionary<IAddress, IAddress> AddressesDict2 { get => _mapper.Map<System.Collections.Generic.Dictionary<IAddress, IAddress>>(_Instance.AddressesDict2); set => _Instance.AddressesDict2 = _mapper.Map<System.Collections.Generic.Dictionary<ProxyInterfaceConsumer.Address, ProxyInterfaceConsumer.Address>>(value); }

    public ProxyInterfaceConsumer.E E { get => _Instance.E; set => _Instance.E = value; }

    public ProxyInterfaceConsumer.IMyInterface MyInterface { get => _Instance.MyInterface; set => _Instance.MyInterface = value; }



    public string Add(string s, string @string)
    {
        string s_ = s;
        string @string_ = @string;
        var result_33785274 = _Instance.Add(s_, @string_);
        return result_33785274;
    }

    public void AddWithParams(params string[] values)
    {
        string[] values_ = values;
        _Instance.AddWithParams(values_);
    }

    public IAddress AddAddress(IAddress a)
    {
        ProxyInterfaceConsumer.Address a_ = _mapper.Map<ProxyInterfaceConsumer.Address>(a);
        var result_9487824 = _Instance.AddAddress(a_);
        return _mapper.Map<IAddress>(result_9487824);
    }

    public void AddAddresses(params ProxyInterfaceConsumer.Address[] addresses)
    {
        ProxyInterfaceConsumer.Address[] addresses_ = addresses;
        _Instance.AddAddresses(addresses_);
    }

    public void In_Out_Ref1(in int a, out int b, ref int c)
    {
        int a_ = a;
        int b_;
        int c_ = c;
        _Instance.In_Out_Ref1(in a_, out b_, ref c_);
        b = b_;
    }

    public int In_Out_Ref2(in IAddress a, out IAddress b, ref IAddress c)
    {
        ProxyInterfaceConsumer.Address a_ = _mapper.Map<ProxyInterfaceConsumer.Address>(a);
        ProxyInterfaceConsumer.Address b_;
        ProxyInterfaceConsumer.Address c_ = _mapper.Map<ProxyInterfaceConsumer.Address>(c);
        var result_30316242 = _Instance.In_Out_Ref2(in a_, out b_, ref c_);
        b = _mapper.Map<IAddress>(_b);
        return result_30316242;
    }

    public void Void()
    {
        _Instance.Void();
    }

    public System.Threading.Tasks.Task Method1Async()
    {
        var result_19162058 = _Instance.Method1Async();
        return result_19162058;
    }

    public System.Threading.Tasks.Task<int> Method2Async()
    {
        var result_1349218716 = _Instance.Method2Async();
        return result_1349218716;
    }

    public System.Threading.Tasks.Task<string?> Method3Async()
    {
        var result_1352687748 = _Instance.Method3Async();
        return result_1352687748;
    }



    public PersonProxy2(ProxyInterfaceConsumer.Person instance)
    {
        _Instance = instance;

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProxyInterfaceConsumer.Address, IAddress>();
            cfg.CreateMap<IAddress, ProxyInterfaceConsumer.Address>();
        }).CreateMapper();

    }

    private readonly IMapper _mapper;
}
}