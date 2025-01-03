using NetAuthManager.Core.User;
using Furion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Expressions;

/// <summary>
/// 权限
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ExpressionParserEx<T>
{
    /// <summary>
    /// 转换条件
    /// </summary>
    /// <param name="permNames">数据权限名</param>
    /// <param name="name">查询视图昵称</param>
    /// <param name="orConditions">或者</param>
    /// <param name="companyColumnName">公司字段</param>
    /// <param name="departmentColumnName">部门字段</param>
    /// <param name="handleUserColumnName">经办人字段</param>
    /// <param name="appUserColumnName">申请人字段</param>
    /// <param name="isAdmin">是否管理员</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> ParserConditionsByPerms(string[] permNames, string name = default,
        List<LinqSelectCondition> orConditions = null,
        string companyColumnName = "CompanyCode", string departmentColumnName = "DeptCode", string handleUserColumnName = "HandleUser", string appUserColumnName = "AppUser")
    {
        //将条件转化成表达式的Body
        var parameter = Expression.Parameter(typeof(T), name);
        var query = ParseExpressionBody(permNames, parameter, companyColumnName, departmentColumnName, handleUserColumnName, appUserColumnName, orConditions: orConditions);
        return Expression.Lambda<Func<T, bool>>(query, parameter);
    }

    private static Expression ParseExpressionBody(string[] permNames, ParameterExpression parameter, string companyColumnName, string departmentColumnName, string handleUserColumnName, string appUserColumnName, List<LinqSelectCondition> orConditions = null)
    {
        if (permNames == null || permNames.Count() == 0)
        {
            return Expression.Constant(false, typeof(bool));
        }
        else if (permNames.Count() == 1 && (permNames[0] == "Execute" || permNames[0] == "ExecuteAll"))
        {
            return Expression.Constant(true, typeof(bool));
        }
        else
        {
            if (permNames.Contains("Execute"))
            {
                var list = permNames.ToList();
                list.Remove("Execute");
                permNames = list.ToArray();
            }
            if (permNames.Contains("ExecuteAll"))
            {
                return Expression.Constant(true, typeof(bool));
            }
            else
            {
                Expression endOut;
                if (permNames.Length == 1)
                {
                    endOut = ParseExpressionBody(permNames[0], parameter, companyColumnName, departmentColumnName, handleUserColumnName, appUserColumnName);
                }
                else
                {
                    Expression first = ParseExpressionBody(permNames.First(), parameter, companyColumnName, departmentColumnName, handleUserColumnName, appUserColumnName);
                    for (var index = 1; index < permNames.Length; index++)
                    {
                        Expression current = ParseExpressionBody(permNames[index], parameter, companyColumnName, departmentColumnName, handleUserColumnName, appUserColumnName);
                        first = Expression.OrElse(first, current);
                    }
                    endOut = first; //此位置可执行其他逻辑操作
                }
                if (orConditions != null)
                {
                    Expression orExpression = ExpressionParser<T>.ParserConditions(orConditions);
                    endOut = Expression.OrElse(endOut, orExpression);
                }
                return endOut;
            }
        }
    }
    private static Expression ParseExpressionBody(string permName, ParameterExpression parameter, string companyColumnName, string departmentColumnName, string handleUserColumnName, string appUserColumnName)
    {
        //当前公司条件
        if (permName == "ExecuteComp") return ParseExpressionComp(companyColumnName, parameter);

        //当前部门条件
        if (permName == "ExecuteDept") return ParseExpressionDept(departmentColumnName, parameter);

        //当前经办人条件
        if (permName == "ExecuteHandle") return ParseExpressionHandleUser(handleUserColumnName, parameter);

        //当前申请人条件
        if (permName == "ExecuteApp") return ParseExpressionAppUser(appUserColumnName, parameter);

        //其他字段条件
        var conditions = ConvertFromPermName(permName);
        return ExpressionParser<T>.ParseExpressionBody(conditions, parameter);
    }
    /// <summary>
    /// 公司条件构建
    /// </summary>
    /// <param name="companyColumnName"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static Expression ParseExpressionComp(string companyColumnName, ParameterExpression parameter)
    {
        //公司编码
        //TODO: 去掉公司条件
        var compannys = new List<dynamic>();// LoginUserInfo.GetCompanys();

        //条件构建
        var condition = new LinqSelectCondition
        {
            Field = companyColumnName,
            Value = string.Join(",", compannys.Select(companny => companny.Code)),
            Op = "in"
        };
        return ExpressionParser<T>.ParseCondition(condition, parameter);
    }
    /// <summary>
    /// 部门条件构建
    /// </summary>
    /// <param name="departmentColumnName"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static Expression ParseExpressionDept(string departmentColumnName, ParameterExpression parameter)
    {
        //部门编码
        //TODO: 去掉部门条件
        var departments = new List<dynamic>();// LoginUserInfo.GetDepartments();

        //条件构建
        var condition = new LinqSelectCondition
        {
            Field = departmentColumnName,
            Value = string.Join(",", departments.Select(companny => companny.Code)),
            Op = "in"
        };
        return ExpressionParser<T>.ParseCondition(condition, parameter);
    }
    /// <summary>
    /// 经办人条件构建
    /// </summary>
    /// <param name="handleUserColumnName"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static Expression ParseExpressionHandleUser(string handleUserColumnName, ParameterExpression parameter)
    {
        //部门编码
        var userInfo = LoginUserInfo.GetLoginUser();

        //条件构建
        var condition = new LinqSelectCondition
        {
            Field = handleUserColumnName,
            Value = userInfo.UserAccount,
            Op = "="
        };
        return ExpressionParser<T>.ParseCondition(condition, parameter);
    }
    /// <summary>
    /// 经办人条件构建
    /// </summary>
    /// <param name="appUserColumnName"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static Expression ParseExpressionAppUser(string appUserColumnName, ParameterExpression parameter)
    {
        //部门编码
        var userInfo = LoginUserInfo.GetLoginUser();

        //条件构建
        var condition = new LinqSelectCondition
        {
            Field = appUserColumnName,
            Value = userInfo.UserAccount,
            Op = "="
        };
        return ExpressionParser<T>.ParseCondition(condition, parameter);
    }
    /// <summary>
    /// 从权限名称中转换
    /// 如：${CompanyCode:9000|A000|9020}
    /// 或：${CompanyCode:9000|A000&BizType:AAA|BBB}
    /// </summary>
    /// <param name="permName">示例：${CompanyCode:9000|A000&BizType:AAA|BBB}</param>
    /// <returns></returns>
    private static IEnumerable<LinqSelectCondition> ConvertFromPermName(string permName, string split = "&", string FieldValueRange = "|")
    {
        var list = new List<LinqSelectCondition>();
        if (string.IsNullOrEmpty(permName))
            throw new Exception("权限操作名称不应该为空，请联系管理员处理！");

        // 定义正则表达式解析模式
        string pattern = @"^\$\{([\s\S]+)\}$";

        // 解析字符串并提取键值对集合
        var match = Regex.Match(permName, pattern, RegexOptions.IgnoreCase);
        if (!match.Success)
            throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

        // 权限
        if (match.Groups.Count < 2)
            throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

        // 内容
        var content = match.Groups[1].Value;
        if (string.IsNullOrEmpty(content))
            throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

        // 分割
        var pairs = content.Split(split); //CompanyCode:9000;A000
        foreach (var pair in pairs)
        {
            string opInner, splitInner;
            if (pair.Contains(":"))
            {
                opInner = "in";
                splitInner = ":";
            }
            else if (pair.Contains("="))
            {
                opInner = "in";
                splitInner = "=";
            }
            else if (pair.Contains("~"))
            {
                opInner = "like";
                splitInner = "~";
            }
            else if (pair.Contains("#"))
            {
                opInner = "between";
                splitInner = "#";
            }
            else
                throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

            var parts = pair.Split(splitInner);
            if (parts.Length != 2)
                throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

            var field = parts[0];
            var value = parts[1] ?? string.Empty;
            if (string.IsNullOrEmpty(field))
                throw new Exception($"权限操作名称无效 {permName} 格式存在问题，请联系管理员处理！");

            if (opInner == "like" && value.Contains(FieldValueRange))
                throw new Exception($"权限操作名称无效 {permName} 格式存在问题，使用 ~(like) 模式，不支持多值 {value}，请联系管理员处理！");

            if (opInner == "between" && value.Split(FieldValueRange).Length != 2)
                throw new Exception($"权限操作名称无效 {permName} 格式存在问题，使用 #(between) 模式，值格式问题 {value}，仅支持2值，如：“1;100”、“1;”、“;100”，请联系管理员处理！");

            list.Add(new LinqSelectCondition
            {
                Field = field,
                Value = value.Replace(FieldValueRange[0], ','),
                Op = opInner
            });
        }
        return list;
    }
}
