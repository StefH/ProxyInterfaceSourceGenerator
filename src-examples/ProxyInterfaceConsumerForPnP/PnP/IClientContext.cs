using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(ClientContext))]
    public partial interface IClientContext : IClientRuntimeContext
    {
    }
}

namespace ProxyInterfaceConsumer.PnP
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class ClientContextProxy
    {
        public void LoadOriginal<T>(T clientObject, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject
        {
            T clientObject_ = clientObject;
            Expression<Func<T, object>>[] retrievals_ = retrievals;
            _Instance.Load<T>(clientObject_, retrievals_);
        }

        public void Load4<TSource, TTarget>(IClientObject clientObject, params Expression<Func<TSource, object>>[] retrievals)
            where TSource : IClientObject
            where TTarget : ClientObject
        {
            TTarget clientObject_ = Mapster.TypeAdapter.Adapt<TTarget>(clientObject);

            _Instance.Load(clientObject_, retrievals.Select(MapExpression<TSource, TTarget>).ToArray());
        }

        private T X<T, R>(R r)
        {
            return default(T)!;
        }

        private static Expression<Func<TTarget, object>> MapExpression<TSource, TTarget>(Expression<Func<TSource, object>> expression)
            where TSource : IClientObject
            where TTarget : ClientObject
        {
            var parameterExpression = Expression.Parameter(typeof(TTarget));

            Expression memberAccessExpression;
            switch (expression.Body)
            {
                case MemberExpression memberExpression:
                    memberAccessExpression = Expression.PropertyOrField(parameterExpression, memberExpression.Member.Name);
                    break;

                case UnaryExpression unaryExpression:
                    var expressionOperand = (MemberExpression)unaryExpression.Operand;
                    memberAccessExpression = Expression.PropertyOrField(parameterExpression, expressionOperand.Member.Name);
                    memberAccessExpression = Expression.Convert(memberAccessExpression, typeof(object));
                    break;

                default:
                    throw new NotSupportedException();

            }

            return Expression.Lambda<Func<TTarget, object>>(memberAccessExpression, parameterExpression);
        }
    }
}