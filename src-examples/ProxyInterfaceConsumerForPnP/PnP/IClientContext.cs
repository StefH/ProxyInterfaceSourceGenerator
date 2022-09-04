using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.BusinessData.MetadataModel;
//using AutoMapper;
//using Mapster;
using Microsoft.SharePoint.Client;
using Nelibur.ObjectMapper;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientContext))]
    public partial interface IClientContext : IClientRuntimeContext
    {
        // public virtual void X();
    }
}

namespace ProxyInterfaceConsumer.PnP
{




    public partial class ClientContextProxy
    {
        public void Test()
        {


            //var web = _mapper.Map<Web>(Web);
            //Load(web, w => w.Lists);

            //     Mapster.TypeAdapterConfig<Web, IWeb>.NewConfig().ConstructUsing(instance_841809920 => new ProxyInterfaceConsumer.PnP.WebProxy(instance_841809920));
            //  Mapster.TypeAdapterConfig<IWeb, Web>.NewConfig().MapWith(proxy1898650104 => ((ProxyInterfaceConsumer.PnP.WebProxy)proxy1898650104)._Instance);


            // Mapster.TypeAdapter.

            var iweb = Mapster.TypeAdapter.Adapt<IWeb>(_Instance.Web);
            var web = Mapster.TypeAdapter.Adapt<Web>(iweb);

            //var mapped = mapper.Map<ProxyInterfaceConsumer.PnP.IWeb>(_Instance.Web);

            //Bind<TSource, TTarget>()


            Load3(Web, w => w.Lists);
        }

        public void LoadOriginal<T>(T clientObject, params System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals)
            where T : Microsoft.SharePoint.Client.ClientObject
        {
            T clientObject_ = clientObject;
            System.Linq.Expressions.Expression<System.Func<T, object>>[] retrievals_ = retrievals;
            _Instance.Load<T>(clientObject_, retrievals_);
        }

        public void Load3(IWeb clientObject, params System.Linq.Expressions.Expression<System.Func<IWeb, object>>[] retrievals)
        {
            var clientObject_ = (WebProxy)clientObject;

            Load(clientObject_._Instance, retrievals.Select(MapMemberLambda<IWeb, Web>).ToArray());
        }

        public void Load4<TSource, TDest>(TSource clientObject, params System.Linq.Expressions.Expression<System.Func<TSource, object>>[] retrievals)
            where TSource : IClientObject
            where TDest : ClientObject
        {
            var clientObject_ = Mapster.TypeAdapter.Adapt<TDest>(clientObject);

            //Load(clientObject);
        }

        public static Expression<Func<TTo, bool>> Converter<TFrom, TTo>(Expression<Func<TFrom, bool>> expression, TTo type)
            where TTo : TFrom
        {
            // here we get the expression parameter the x from (x) => ....
            var parameterName = expression.Parameters.First().Name;
            // create the new parameter from the correct type
            ParameterExpression parameter = Expression.Parameter(typeof(TTo), parameterName);
            // asigne to allow the visit from or visitor
            Expression body = new ConvertVisitor(parameter).Visit(expression.Body);
            // recreate the expression
            return Expression.Lambda<Func<TTo, bool>>(body, parameter);
        }

        

        static Expression<Func<TTarget, object>> MapMemberLambda<TSource, TTarget>(Expression<Func<TSource, object>> expression)
        {
            ParameterExpression objectParam = Expression.Parameter(typeof(TTarget), "x");

            PropertyInfo propertyOrField;
            switch (expression.Body)
            {
                case MemberExpression memberExpression:
                    propertyOrField = (PropertyInfo) memberExpression.Member;
                    break;

                case UnaryExpression unaryExpression:
                    var expressionOperand = (MemberExpression) unaryExpression.Operand;
                    propertyOrField = (PropertyInfo) expressionOperand.Member;
                    break;

                default:
                    throw new NotSupportedException();

            }

            Expression memberAccessExpression = Expression.PropertyOrField(objectParam, propertyOrField.Name);
            if (propertyOrField.PropertyType != typeof(object))
            {
                memberAccessExpression = Expression.Convert(memberAccessExpression, typeof(object));
            }

            return Expression.Lambda<Func<TTarget, object>>(memberAccessExpression, objectParam);
        }
    }

    public class ConvertVisitor : ExpressionVisitor
    {
        private ParameterExpression Parameter;

        public ConvertVisitor(ParameterExpression parameter)
        {
            Parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression item)
        {
            // we just check the parameter to return the new value for them
            if (!item.Name.Equals(Parameter.Name))
                return item;
            return Parameter;
        }
    }
}