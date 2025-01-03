using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetAuthManager.Core.Expressions;

/// <summary>
/// 构建查询参数
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ExpressionParser<T>
{
    /// <summary>
    /// 转换条件
    /// </summary>
    /// <param name="conditions">查询条件</param>
    /// <param name="name">查询视图昵称</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> ParserConditions(IEnumerable<LinqSelectCondition> conditions, string? name = default)
    {
        //将条件转化成表达式的Body
        var parameter = Expression.Parameter(typeof(T), name);
        var query = ParseExpressionBody(conditions, parameter);
        return Expression.Lambda<Func<T, bool>>(query, parameter);
    }

    public static Expression ParseExpressionBody(IEnumerable<LinqSelectCondition> conditions, ParameterExpression parameter)
    {
        if (conditions == null || conditions.Count() == 0)
        {
            return Expression.Constant(true, typeof(bool));
        }
        else if (conditions.Count() == 1)
        {
            return ParseCondition(conditions.First(), parameter);
        }
        else
        {
            Expression left = ParseCondition(conditions.First(), parameter);
            Expression right = ParseExpressionBody(conditions.Skip(1), parameter);
            return Expression.AndAlso(left, right); //此位置可执行其他逻辑操作
        }
    }
    
    //对查询条件进行处理
    public static Expression ParseCondition(LinqSelectCondition condition, ParameterExpression parameter)
    {
        //类型
        if (condition.Type == null)
        {
            var ptype = GetPropertyType(condition.Field);
            if (ptype != null) condition.Type = ptype.Name;
        }

        ParameterExpression p = parameter;
        //对值进行转换处理
        object convertValue = condition.Value;
        if (condition.Type != null
            && condition.Operator != LinqSelectOperator.InWithContains
            && condition.Operator != LinqSelectOperator.InWithEqual
            && condition.Operator != LinqSelectOperator.Between)
        {
            if (condition.Type.ToUpper() == "DATETIME")
            {
                convertValue = System.Convert.ToDateTime(condition.Value);
            }
            else if (condition.Type.ToUpper() == "INT")
            {
                convertValue = System.Convert.ToInt32(condition.Value);
            }
            else if (condition.Type.ToUpper() == "LONG")
            {
                convertValue = System.Convert.ToInt64(condition.Value);
            }
            else if (condition.Type.ToUpper() == "DOUBLE")
            {
                convertValue = System.Convert.ToDouble(condition.Value);
            }
            else if (condition.Type.ToUpper() == "DECIMAL")
            {
                convertValue = System.Convert.ToDecimal(condition.Value);
            }
            else if (condition.Type.ToUpper() == "BOOL" || condition.Type.ToUpper() == "BOOLEAN")
            {
                convertValue = System.Convert.ToBoolean(condition.Value);
            }
        }
        Expression value = Expression.Constant(convertValue);
        Expression key = Expression.Property(p, condition.Field);
        switch (condition.Operator)
        {
            case LinqSelectOperator.Contains:
                if (string.IsNullOrEmpty(condition.Value)) return Expression.Constant(true, typeof(bool));
                return Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
            case LinqSelectOperator.Equal:
                return Expression.Equal(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.Greater:
                return Expression.GreaterThan(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.GreaterEqual:
                return Expression.GreaterThanOrEqual(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.Less:
                return Expression.LessThan(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.LessEqual:
                return Expression.LessThanOrEqual(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.NotEqual:
                return Expression.NotEqual(key, Expression.Convert(value, key.Type));
            case LinqSelectOperator.InWithEqual:
                return ParaseIn(key, condition, true);
            case LinqSelectOperator.InWithContains:
                return ParaseIn(key, condition, false);
            case LinqSelectOperator.Between:
                return ParaseBetween(key, condition);
            default:
                throw new NotImplementedException("不支持此操作");
        }
    }
    //对查询“Between"的处理
    private static Expression ParaseBetween(Expression key, LinqSelectCondition conditions)
    {
        //ParameterExpression p = parameter;
        //Expression key = Expression.Property(p, conditions.Field);
        var valueArr = conditions.Value.Split(',');
        if (valueArr.Length != 2)
        {
            throw new NotImplementedException("ParaseBetween参数错误");
        }

        Expression expression = Expression.Constant(true, typeof(bool));

        object conValue1 = null;
        object conValue2 = null;
        Type keyType = key.Type;
        if (conditions.Type?.ToUpper() == "INT")
        {
            if (!string.IsNullOrEmpty(valueArr[0]))
                conValue1 = Convert.ToInt32(valueArr[0]);
            if (!string.IsNullOrEmpty(valueArr[1]))
                conValue2 = Convert.ToInt32(valueArr[1]);
            //keyType = typeof(int);
        }
        else if (conditions.Type?.ToUpper() == "LONG")
        {
            if (!string.IsNullOrEmpty(valueArr[0]))
                conValue1 = Convert.ToInt64(valueArr[0]);
            if (!string.IsNullOrEmpty(valueArr[1]))
                conValue2 = Convert.ToInt64(valueArr[1]);
            //keyType = typeof(long);
        }
        else if (conditions.Type?.ToUpper() == "DECIMAL")
        {
            if (!string.IsNullOrEmpty(valueArr[0]))
                conValue1 = Convert.ToDecimal(valueArr[0]);
            if (!string.IsNullOrEmpty(valueArr[1]))
                conValue2 = Convert.ToDecimal(valueArr[1]);
            //keyType = typeof(decimal);
        }
        else if (conditions.Type?.ToUpper() == "DATETIME")
        {
            if (!string.IsNullOrEmpty(valueArr[0]))
                conValue1 = Convert.ToDateTime(valueArr[0]);
            if (!string.IsNullOrEmpty(valueArr[1]))
            {
                var tempDate = Convert.ToDateTime(valueArr[1]);
                if (tempDate == tempDate.Date)
                {
                    tempDate=tempDate.AddDays(1).AddSeconds(-1);
                }
                conValue2 = tempDate;
            }
            //keyType = typeof(DateTime);
        }
        else
        {
            throw new NotImplementedException("ParaseBetween参数只能为数字/日期");
        }

        if (conValue1 != null && conValue2 != null)
        {
            //开始位置
            Expression startvalue = Expression.Constant(conValue1);
            Expression start = Expression.GreaterThanOrEqual(key, Expression.Convert(startvalue, keyType));

            Expression endvalue = Expression.Constant(conValue2);
            Expression end = Expression.LessThanOrEqual(key, Expression.Convert(endvalue, keyType));
            return Expression.AndAlso(start, end);
        }
        else
        {
            if (conValue1 != null)
            {
                //开始位置
                Expression startvalue = Expression.Constant(conValue1);
                Expression start = Expression.GreaterThanOrEqual(key, Expression.Convert(startvalue, keyType));
                return start;
            }
            else
            {
                Expression endvalue = Expression.Constant(conValue2);
                Expression end = Expression.LessThanOrEqual(key, Expression.Convert(endvalue, keyType));
                return end;
            }
        }
    }
    //对查询“in"的处理
    private static Expression ParaseIn(Expression key, LinqSelectCondition conditions, bool isEqual)
    {
        var valueArr = conditions.Value.Split(',');
        Expression expression = Expression.Constant(false, typeof(bool));
        foreach (var itemVal in valueArr)
        {
            object conValue = itemVal;
            Type keyType = key.Type;
            if (conditions.Type?.ToUpper() == "INT")
            {
                conValue = System.Convert.ToInt32(itemVal);
            }
            else if (conditions.Type?.ToUpper() == "LONG")
            {
                conValue = System.Convert.ToInt64(itemVal);
            } 
            Expression value = Expression.Constant(conValue);
            Expression right;
            if (isEqual)
                right = Expression.Equal(key, Expression.Convert(value, keyType));
            else
                right = Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
            expression = Expression.OrElse(expression, right);
        }
        return expression;
    }
    /// <summary>
    /// 获取泛型类的指定名称字段
    /// </summary>
    private static PropertyInfo? GetPropertyInfo(string field)
    {
        Type typeT = typeof(T);
        var propertyInfos = typeT.GetProperties();
        return propertyInfos.FirstOrDefault(p => p.Name.ToUpper() == field.ToUpper());
    }
    private static Type GetPropertyType(string field)
    {
        var property = GetPropertyInfo(field);
        if (property == null)
            throw new Exception($"不存在的查询字段 {field}");
        var canNull = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        if (canNull)
        {
            return property.PropertyType.GetGenericArguments()[0];
        }
        else
        {
            return property.PropertyType;
        }
    }
}