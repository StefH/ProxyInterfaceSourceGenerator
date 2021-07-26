using System;
using AutoMapper;

namespace ProxyInterfaceConsumer
{
    public class PersonProxy2
    {
        public ProxyInterfaceConsumer.Person _Instance { get; }

        public int Id { get => _Instance.Id; }

        public long? NullableLong { get => _Instance.NullableLong; }

        public string Name { get => _Instance.Name; set => _Instance.Name = value; }

        
        public IAddress Address { get => _mapper.Map<IAddress>(_Instance.Address); set => _Instance.Address = _mapper.Map<ProxyInterfaceConsumer.Address>(value); }

        public System.Collections.Generic.List<IAddress> AddressesLIst { get => _mapper.Map<System.Collections.Generic.List<IAddress>>(_Instance.AddressesLIst); set => _Instance.AddressesLIst = _mapper.Map<System.Collections.Generic.List<ProxyInterfaceConsumer.Address>>(value); }

        public System.Collections.Generic.Dictionary<string, IAddress> AddressesDict { get => _mapper.Map<System.Collections.Generic.Dictionary<string, IAddress>>(_Instance.AddressesDict); set => _Instance.AddressesDict = _mapper.Map<System.Collections.Generic.Dictionary<string, ProxyInterfaceConsumer.Address>>(value); }

        public ProxyInterfaceConsumer.E E { get => _Instance.E; set => _Instance.E = value; }

        public ProxyInterfaceConsumer.IMyInterface MyInterface { get => _Instance.MyInterface; set => _Instance.MyInterface = value; }



        public int Add(string s)
        {
            string s_dbccfd45ed944f58b12d83a4f907aa6c = s;
            var result_caf8bee7109d4b77891b141c495b63ff = _Instance.Add(s_dbccfd45ed944f58b12d83a4f907aa6c);
            return result_caf8bee7109d4b77891b141c495b63ff;
        }

       

        public IAddress AddAddress(IAddress a)
        {
            ProxyInterfaceConsumer.Address a_23d6262793aa4c90b77bb7a9d46710b2 = _mapper.Map<ProxyInterfaceConsumer.Address>(a);
            var result_cd3011159452417bb585e0acfaeefddc = _Instance.AddAddress(a_23d6262793aa4c90b77bb7a9d46710b2);
            return _mapper.Map<IAddress>(result_cd3011159452417bb585e0acfaeefddc);
        }

        public void In_Out_Ref1(in int a, out int b, ref int c)
        {
            int a_88b067399c9641d69ebd8f795ddfa7ee = a;
            int b_9a4c5b7b7e4c427dbb4779f658602356;
            int c_49084012db6e47f0b03626886b8b7848 = c;
            _Instance.In_Out_Ref1(in a_88b067399c9641d69ebd8f795ddfa7ee, out b_9a4c5b7b7e4c427dbb4779f658602356, ref c_49084012db6e47f0b03626886b8b7848);
            b = b_9a4c5b7b7e4c427dbb4779f658602356;
        }

        public void In_Out_Ref2(in IAddress a, out IAddress b, ref IAddress c)
        {
            ProxyInterfaceConsumer.Address a_e5af7467b9d24729a95a861a8cc87f27 = _mapper.Map<ProxyInterfaceConsumer.Address>(a);
            ProxyInterfaceConsumer.Address b_3a7ae9dbab3344bc9f9736b113198331;
            ProxyInterfaceConsumer.Address c_afcd2b8abb1a4b7eae2a10656744c28a = _mapper.Map<ProxyInterfaceConsumer.Address>(c);
            _Instance.In_Out_Ref2(in a_e5af7467b9d24729a95a861a8cc87f27, out b_3a7ae9dbab3344bc9f9736b113198331, ref c_afcd2b8abb1a4b7eae2a10656744c28a);
            b = _mapper.Map<IAddress>(b_3a7ae9dbab3344bc9f9736b113198331);
        }

        public void Void()
        {
            _Instance.Void();
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