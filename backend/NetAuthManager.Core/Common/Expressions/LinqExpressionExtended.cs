using Furion.FriendlyException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Expressions;

public static class LinqExpressionExtended
{
    //扩展查询
    //public static IQueryable<T> QueryConditions<T>(this IQueryable<T> query, IEnumerable<LinqSelectCondition> conditions)
    //{
    //    var filter = ExpressionParser<T>.ParserConditions(conditions);
    //    return query.Where(filter);
    //}

    //扩展多条件排序
    //public static IQueryable<T> OrderConditions<T>(this IQueryable<T> query, IEnumerable<LinqOrderCondition> orderConditions)
    //{
    //    foreach (var orderinfo in orderConditions)
    //    {
    //        var parameter = Expression.Parameter(typeof(T));
    //        Expression propertySelector = parameter;
    //        if (orderinfo.ParentFields != null && orderinfo.ParentFields.Count > 0)
    //        {
    //            foreach (var parent in orderinfo.ParentFields)
    //            {
    //                propertySelector = Expression.Property(propertySelector, parent);
    //            }
    //            propertySelector = Expression.Property(propertySelector, orderinfo.Field);
    //        }
    //        else
    //        {
    //            propertySelector = Expression.Property(propertySelector, orderinfo.Field);
    //        }
    //        if (propertySelector.Type == typeof(long)) //int64不能直接转换为object
    //        {
    //            var orderby = Expression.Lambda<Func<T, long>>(propertySelector, parameter);
    //            //var funcType = typeof(Func<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
    //            //var orderby = Expression.Lambda(funcType,propertySelector, parameter);
    //            if (orderinfo.OrderType == OrderByType.Desc)
    //                query = query.OrderByDescending(orderby);
    //            else
    //                query = query.OrderBy(orderby);
    //        }
    //        else if (propertySelector.Type == typeof(int)) //int64不能直接转换为object
    //        {
    //            var orderby = Expression.Lambda<Func<T, int>>(propertySelector, parameter);
    //            //var funcType = typeof(Func<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
    //            //var orderby = Expression.Lambda(funcType,propertySelector, parameter);
    //            if (orderinfo.OrderType == OrderByType.Desc)
    //                query = query.OrderByDescending(orderby);
    //            else
    //                query = query.OrderBy(orderby);
    //        }
    //        else if (propertySelector.Type == typeof(DateTime))
    //        {
    //            var orderby = Expression.Lambda<Func<T, DateTime>>(propertySelector, parameter);
    //            if (orderinfo.OrderType == OrderByType.Desc)
    //                query = query.OrderByDescending(orderby);
    //            else
    //                query = query.OrderBy(orderby);
    //        }
    //        else if (propertySelector.Type == typeof(TimeSpan))
    //        {
    //            var orderby = Expression.Lambda<Func<T, TimeSpan>>(propertySelector, parameter);
    //            if (orderinfo.OrderType == OrderByType.Desc)
    //                query = query.OrderByDescending(orderby);
    //            else
    //                query = query.OrderBy(orderby);
    //        }
    //        else
    //        {
    //            var orderby = Expression.Lambda<Func<T, object>>(propertySelector, parameter);
    //            if (orderinfo.OrderType == OrderByType.Desc)
    //                query = query.OrderByDescending(orderby);
    //            else
    //                query = query.OrderBy(orderby);
    //        }

    //    }
    //    return query;
    //}

    /// <summary>
    /// 扩展多条件排序
    /// </summary>
    /// <typeparam name="TEntry"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="orderConditions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IOrderedQueryable<TEntry> OrderBy<TEntry>(this IQueryable<TEntry> queryable, List<LinqOrderCondition> orderConditions, string name = default)
    {
        if (orderConditions == null || orderConditions.Count() == 0) return queryable.OrderBy(x => 1);

        var parameter = Expression.Parameter(typeof(TEntry), name);

        IOrderedQueryable<TEntry> orderQueryable = null;
        foreach (var orderCondition in orderConditions)
        {
            if (orderQueryable == null)
            {
                orderQueryable = OrderBy(queryable, orderCondition, parameter);
            }
            else
            {
                orderQueryable = OrderBy(orderQueryable, orderCondition, parameter);
            }
        }
        return orderQueryable;
    }
    private static IOrderedQueryable<TEntry> OrderBy<TEntry>(this IQueryable<TEntry> queryable, LinqOrderCondition orderinfo, ParameterExpression parameter)
    {
        Expression propertySelector = Expression.Property(parameter, orderinfo.Field);
        if (propertySelector.Type == typeof(long)) //int64不能直接转换为object
        {
            var orderby = Expression.Lambda<Func<TEntry, long>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(int)) //int64不能直接转换为object
        {
            var orderby = Expression.Lambda<Func<TEntry, int>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(int?)) //int64不能直接转换为object
        {
            var orderby = Expression.Lambda<Func<TEntry, int?>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(decimal)) //int64不能直接转换为object
        {
            var orderby = Expression.Lambda<Func<TEntry, decimal>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(decimal?)) //int64不能直接转换为object
        {
            var orderby = Expression.Lambda<Func<TEntry, decimal?>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(DateTime))
        {
            var orderby = Expression.Lambda<Func<TEntry, DateTime>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(DateTime?))
        {
            var orderby = Expression.Lambda<Func<TEntry, DateTime?>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(TimeSpan))
        {
            var orderby = Expression.Lambda<Func<TEntry, TimeSpan>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(bool?))
        {
            var orderby = Expression.Lambda<Func<TEntry, bool?>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else if (propertySelector.Type == typeof(bool))
        {
            var orderby = Expression.Lambda<Func<TEntry, bool>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        else
        {
            var orderby = Expression.Lambda<Func<TEntry, object>>(propertySelector, parameter);
            if (orderby != null)
            {
                return queryable.OrderBy(orderby, orderinfo.Order);
                //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
            }
        }
        return queryable.OrderBy(x => 1);
    }

    /// <summary>
    /// 按照指定表达式、排序方向进行排序
    /// </summary>
    /// <typeparam name="TEntry"></typeparam>
    /// <typeparam name="Tkey"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="orderbyKey"></param>
    /// <param name="orderType"></param>
    private static IOrderedQueryable<TEntry> OrderBy<TEntry, Tkey>(this IQueryable<TEntry> queryable, Expression<Func<TEntry, Tkey>> orderbyKey, OrderByType orderType)
    {
        //queryable.OrderBy(orderinfo.Field + " " + orderinfo.Order.ToString());
        switch (orderType)
        {
            case OrderByType.Asc:
                return queryable.OrderBy(orderbyKey);
            case OrderByType.Desc:
                return queryable.OrderByDescending(orderbyKey);
        }
        return null;
    }
}
