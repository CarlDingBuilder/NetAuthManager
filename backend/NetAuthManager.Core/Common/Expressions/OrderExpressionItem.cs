using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Expressions;

/// <summary>
/// 排序项
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class OrderExpressionItem<TEntity>
{
#nullable enable
    public Expression<Func<TEntity, dynamic>>? OrderByExpression { get; set; }

    public OrderByType OrderByType { get; set; }

    public OrderExpressionItem(Expression<Func<TEntity, dynamic>>? orderByExpression, OrderByType orderByType)
    {
        OrderByExpression = orderByExpression;
        OrderByType = orderByType;
    }
#nullable disable
}
