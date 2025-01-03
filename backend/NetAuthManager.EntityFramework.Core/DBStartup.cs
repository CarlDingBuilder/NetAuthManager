using NetAuthManager.Core.Helper;
using Furion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NetAuthManager.Core.Entities;
using Furion.DatabaseAccessor;

namespace NetAuthManager.EntityFramework.Core;

/// <summary>
/// 数据库配置
/// </summary>
public static class DBStartup
{
    /// <summary>
    /// 添加数据库上下文
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSqlServerSetup(this IServiceCollection services, IConfiguration configuration)
    {
        //链接字符串
        var connectionConfig = ConfigHelper.GetConnectionConfig();
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>(
                connectionMetadata: connectionConfig.ConnectionString
                //, interceptors: new IInterceptor[] {
                //    new SqlCommandProfilerInterceptor()
                //}
            );
        }, "NetAuthManager.Database.Migrations");
    }

    /// <summary>
    /// 添加数据库上下文
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSqliteSetup(this IServiceCollection services, IConfiguration configuration)
    {
        //链接字符串
        var connectionConfig = ConfigHelper.GetConnectionConfig();
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>(
                connectionMetadata: connectionConfig.ConnectionString
            //, interceptors: new IInterceptor[] {
            //    new SqlCommandProfilerInterceptor()
            //}
            );
        }, "NetAuthManager.Database.Migrations");
    }

    /// <summary>
    /// 添加数据库上下文
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddOracleSetup(this IServiceCollection services, IConfiguration configuration)
    {
        //链接字符串
        var connectionConfig = ConfigHelper.GetConnectionConfig();
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>(
                connectionMetadata: connectionConfig.ConnectionString,
                optionBuilder: (ses, opt) =>
                {
                    switch (connectionConfig.DBVersion)
                    {
                        case "19":
                        case "DatabaseVersion19":
                            opt.UseOracle(sqlOptions => sqlOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
                            break;
                        case "21":
                        case "DatabaseVersion21":
                            opt.UseOracle(sqlOptions => sqlOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21));
                            break;
                        case "23":
                        case "DatabaseVersion23":
                            opt.UseOracle(sqlOptions => sqlOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion23));
                            break;
                    }
                }
                //, interceptors: new IInterceptor[] {
                //    new SqlCommandProfilerInterceptor()
                //}
            );
        }, "NetAuthManager.Database.Migrations");
    }
}
