using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

// ReSharper disable once CheckNamespace
namespace ProxyInterfaceConsumerForPnP.Interfaces;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class ClientContextProxy
{
    public Task ExecuteQueryRetryAsync(
        Interfaces.IClientRuntimeContext clientContext,
        int retryCount = 10,
        string? userAgent = null
    )
    {
        ClientRuntimeContext clientObject_ = Mapster.TypeAdapter.Adapt<ClientRuntimeContext>(
            clientContext
        );
        return clientObject_.ExecuteQueryRetryAsync(retryCount, userAgent);
    }

    public void LoadOriginal<T>(T clientObject, params Expression<Func<T, object>>[] retrievals)
        where T : ClientObject
    {
        T clientObject_ = clientObject;
        Expression<Func<T, object>>[] retrievals_ = retrievals;
        _Instance.Load<T>(clientObject_, retrievals_);
    }

    public void Load<TSource, TTarget>(
        Interfaces.IClientObject clientObject,
        params Expression<Func<TSource, object>>[] retrievals
    )
        where TSource : Interfaces.IClientObject
        where TTarget : ClientObject
    {
        TTarget clientObject_ = Mapster.TypeAdapter.Adapt<TTarget>(clientObject);
        Expression<Func<TTarget, object>>[] retrievals_ = retrievals
            .Select(MapExpression<TSource, TTarget>)
            .ToArray();

        _Instance.Load(clientObject_, retrievals_);
    }

    private static Expression<Func<TTarget, object>> MapExpression<TSource, TTarget>(
        Expression<Func<TSource, object>> expression
    )
        where TSource : Interfaces.IClientObject
        where TTarget : ClientObject
    {
        var parameterExpression = Expression.Parameter(typeof(TTarget));

        Expression memberAccessExpression;
        switch (expression.Body)
        {
            case MemberExpression memberExpression:
                memberAccessExpression = Expression.PropertyOrField(
                    parameterExpression,
                    memberExpression.Member.Name
                );
                break;

            case UnaryExpression unaryExpression:
                var expressionOperand = (MemberExpression)unaryExpression.Operand;
                memberAccessExpression = Expression.PropertyOrField(
                    parameterExpression,
                    expressionOperand.Member.Name
                );
                memberAccessExpression = Expression.Convert(memberAccessExpression, typeof(object));
                break;

            default:
                throw new NotSupportedException();
        }

        return Expression.Lambda<Func<TTarget, object>>(
            memberAccessExpression,
            parameterExpression
        );
    }
}
