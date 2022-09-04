using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Mapster;
using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientContext))]
    public partial interface IClientContext: IClientRuntimeContext
    {
        // public virtual void X();
    }
}

namespace ProxyInterfaceConsumer.PnP
{
    public class CustomResolver : IValueResolver<object, object, object>
    {
        public object Resolve(object source, object destination, object member, ResolutionContext context)
        {
            try
            {
                return member;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public class MyConverter : ITypeConverter<object, object>
    {
        public object Convert(object source, object destination, ResolutionContext context)
        {
            return System.Convert.ToDateTime(source);
        }
    }

    public class MyWebConverter : ITypeConverter<Web, IWeb>
    {
        public IWeb Convert(Web source, IWeb destination, ResolutionContext context)
        {
            return new ProxyInterfaceConsumer.PnP.WebProxy(source);
        }
    }

    public partial class ClientContextProxy
    {
        public void Test()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                //cfg.ForAllMaps((map, expression) =>
                //{
                //    expression.ForAllMembers(configurationExpression =>
                //        configurationExpression.PreCondition((o, o1, arg3) =>
                //        {
                //            return true;
                //        })
                //        );
                //});

                // CreateMap<TSource, TDestination>();
                cfg.CreateMap<Microsoft.SharePoint.Client.Web, ProxyInterfaceConsumer.PnP.IWeb>()

                    .ConstructUsing((instance_841809920, context) =>
                    {
                        try
                        {
                            var p = new ProxyInterfaceConsumer.PnP.WebProxy(instance_841809920);
                            return p;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                    })

                    //.ConvertUsing<MyWebConverter>()

                    .ForAllMembers(opt =>
                    {
                        //opt.MapFrom(x =>x , );
                        //opt.ConvertUsing<MyConverter, IWeb>( x=> x);

                        opt.PreCondition((src, dest, context) =>
                        {
                            try
                            {
                                var x = src != null;
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        });

                        opt.MapAtRuntime();

                        opt.MapFrom<CustomResolver>();
                    })


                    //.ConstructUsing(instance_841809920 => new ProxyInterfaceConsumer.PnP.WebProxy(instance_841809920))
                    //.ForAllMembers(opt => {
                    //    opt.PreCondition((src, dest, context) =>
                    //    {
                    //        try
                    //        {
                    //            var x = src != null;
                    //            return true;
                    //        }
                    //        catch
                    //        {
                    //            return false;
                    //        }
                    //    });
                    //})

                    ;


                //cfg.CreateMap<ProxyInterfaceConsumer.PnP.IWeb, Microsoft.SharePoint.Client.Web>()
                //    .ConstructUsing(proxy1898650104 => ((ProxyInterfaceConsumer.PnP.WebProxy)proxy1898650104)._Instance)
                //    .ForAllMembers(opt => {
                //        opt.PreCondition((src, dest, context) =>
                //        {
                //            try
                //            {
                //                var x = src != null;
                //                return true;
                //            }
                //            catch
                //            {
                //                return false;
                //            }
                //        });
                //    });
            }).CreateMapper();

            //var web = _mapper.Map<Web>(Web);
            //Load(web, w => w.Lists);

            TypeAdapterConfig<Web, IWeb>.NewConfig().ConstructUsing(instance_841809920 => new ProxyInterfaceConsumer.PnP.WebProxy(instance_841809920));

            TypeAdapterConfig<IWeb, Web>.NewConfig().MapWith(proxy1898650104 => ((ProxyInterfaceConsumer.PnP.WebProxy)proxy1898650104)._Instance);

            var iweb =  _Instance.Web.Adapt<IWeb>();
            var web = iweb.Adapt<Web>();

            //var mapped = mapper.Map<ProxyInterfaceConsumer.PnP.IWeb>(_Instance.Web);

            Load3(Web, w => w.Lists);
        }

        public void LoadOriginal<T>(T clientObject, params System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals) where T : Microsoft.SharePoint.Client.ClientObject
        {
            T clientObject_ = clientObject;
            System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals_ = retrievals;
            _Instance.Load<T>(clientObject_, retrievals_);
        }

        //public void Load2(IClientObject clientObject, params Expression<Func<IClientObject, object>>[] retrievals)
        //{
        //    ClientObject clientObject_ = _mapper.Map<ClientObject>(clientObject);
        //    Expression<Func<ClientObject, object>>[] retrievals_ = _mapper.Map<Expression<Func<ClientObject, object>>[]>(retrievals);

        //    _Instance.Load(clientObject_, retrievals_);
        //}

        public void Load3(IWeb clientObject, params System.Linq.Expressions.Expression<System.Func<IWeb, object>>[] retrievals)
        {
            var clientObject_ = (WebProxy) clientObject;

            //Expression<Func<WebProxy, object>>[] retrievals_ = _mapper.Map<Expression<Func<WebProxy, object>>[]>(retrievals);

            Load(clientObject_._Instance, null);
        }
    }
}