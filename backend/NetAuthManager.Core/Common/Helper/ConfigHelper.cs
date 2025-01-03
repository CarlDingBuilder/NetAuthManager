using Furion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Helper;

/// <summary>
/// 配置管理
/// </summary>
public static class ConfigHelper
{
    /// <summary>
    /// 读取 appsettings.json 中的 ConnectionConfigs 配置节点
    /// </summary>
    public static ConnectionConfig GetConnectionConfig(string name = "Default")
    {
        var connectionConfigs = App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs");
        if (connectionConfigs.Count == 0)
            throw new Exception("数据库连接未配置，系统无法正常运行");

        var connectionConfig = connectionConfigs.FirstOrDefault(x => x.ConfigId == name);
        if (connectionConfig == null)
            throw new Exception($"数据库连接 {name} 未配置，系统无法正常运行");

        return connectionConfig;
    }
}
