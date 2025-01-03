using NetAuthManager.Core.Common.Extended;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Helper;
using NetAuthManager.Core.Models;
using NetAuthManager.EntityFramework.Core.DbFunctions;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace NetAuthManager.EntityFramework.Core;

//用于配置 Sqlite
[AppDbContext("NetAuthManager", DbProvider.Sqlite)]
//用于配置 SqlServer
//[AppDbContext("NetAuthManager", DbProvider.SqlServer)]
//用于配置 Oracle
//[AppDbContext("NetAuthManager", DbProvider.Oracle)]
//这里就没有建立三个DbContext了，如果项目中药同时支持不同数据库，可以自定义
public class DefaultDbContext : AppDbContext<DefaultDbContext>
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<SysUser> Users { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    public DbSet<SysMenu> Menus { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public DbSet<SysRole> Roles { get; set; }

    /// <summary>
    /// 角色成员
    /// </summary>
    public DbSet<SysRoleMember> RoleMembers { get; set; }

    /// <summary>
    /// 角色组
    /// </summary>
    public DbSet<SysRoleGroup> RoleGroups { get; set; }

    /// <summary>
    /// 角色组成员
    /// </summary>
    public DbSet<SysRoleGroupMember> RoleGroupMembers { get; set; }

    /// <summary>
    /// 资源操作
    /// </summary>
    public DbSet<SysResourcePerm> ResourcePerms { get; set; }

    /// <summary>
    /// 资源操作权限
    /// </summary>
    public DbSet<SysResourcePermAcl> ResourcePermAcls { get; set; }

    /// <summary>
    /// 重载不全局跟踪
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        //修改实体是否自动
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }

    /// <summary>
    /// 模型创建
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var config = ConfigHelper.GetConnectionConfig();
        if (config.DbType == DbType.Oracle)
        {
            if (string.IsNullOrEmpty(config.DBVersion))
            {
                // bool 类型处理
                modelBuilder.UseNumberForBoolean();
            }

            // 将自定义 OracleDbFunctions 加入到模型中  
            modelBuilder.HasDbFunction(() => OracleDbFunctions.DateDiffInMinutes(default, default))
                .HasTranslation(args =>
                {
                    // 假设 args 实际类型支持这些常见接口属性  
                    var finishAtExpr = args[1] as ColumnExpression;
                    var receiveAtExpr = args[0] as ColumnExpression;

                    // 获取表别名和列名  
                    //var finishAtTableAlias = (finishAtExpr.Table as TableExpression).Name;
                    var finishAtColumnName = finishAtExpr.Name;

                    //var receiveAtTableAlias = (receiveAtExpr.Table as TableExpression).Name;
                    var receiveAtColumnName = receiveAtExpr.Name;

                    // 构建 SQL 字符串  
                    var finishAtColumn = $"\"{finishAtColumnName}\""; // $"\"{finishAtTableAlias}\".\"{finishAtColumnName}\"";
                    var receiveAtColumn = $"\"{receiveAtColumnName}\""; //$"\"{receiveAtTableAlias}\".\"{receiveAtColumnName}\"";

                    var castFinishAt = new SqlFragmentExpression($"CAST({finishAtColumn} AS DATE)");
                    var castReceiveAt = new SqlFragmentExpression($"CAST({receiveAtColumn} AS DATE)");

                    // 执行日期减法  
                    var subtractExpression = new SqlBinaryExpression(
                        ExpressionType.Subtract,
                        castFinishAt,
                        castReceiveAt,
                        typeof(double), // 结果类型  
                        null
                    );

                    // 将时间差转换为分钟  
                    return new SqlBinaryExpression(
                        ExpressionType.Multiply,
                        subtractExpression,
                        new SqlConstantExpression(Expression.Constant(1440), null),
                        typeof(int), // 最终结果为 double 类型  
                        null
                    );
                });

            //// decimal 类型处理，不需要，因为只能固定长度
            //modelBuilder.UseDecimal();
        }

        // 定义种子数据
        modelBuilder.Entity<SysUser>().HasData(
            new SysUser
            {
                Account = "sa",
                Name = "超级管理员",
                Password = "e10adc3949ba59abbe56e057f20f883e",
                CreatorAccount = "sa",
                CreatorName = "超级管理员",
            }
        );
    }
}
