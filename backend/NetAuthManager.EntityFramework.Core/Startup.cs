using Furion;
using Furion.DatabaseAccessor;
using Microsoft.Extensions.DependencyInjection;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Helper;
using NetAuthManager.Core.Models;
using System;
using System.Linq;

namespace NetAuthManager.EntityFramework.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var connectionConfig = ConfigHelper.GetConnectionConfig();
        switch(connectionConfig.DbType)
        {
            case DbType.SqlServer:
                //添加 Sql Server 注入
                services.AddSqlServerSetup(App.Configuration);
                break;

            case DbType.Sqlite:
                //添加 Sqlite 注入
                services.AddSqliteSetup(App.Configuration);
                break;

            case DbType.Oracle:
                //添加 Oracle 注入
                services.AddOracleSetup(App.Configuration);
                break;
        }

        // 检查是否已有种子数据  
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<DefaultDbContext>();
        if (!dbContext.Users.Any(u => u.Account == "sa"))
        {
            dbContext.Users.Add(new SysUser
            {
                Account = "sa",
                Name = "超级管理员",
                Password = "e10adc3949ba59abbe56e057f20f883e",
                CreatorAccount = "sa",
                CreatorName = "超级管理员",
                CreatedTime = DateTime.UtcNow
            });

            dbContext.SaveChanges();
        }
    }
}
