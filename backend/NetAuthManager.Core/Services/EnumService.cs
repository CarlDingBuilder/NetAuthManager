using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Services;

/// <summary>
/// 枚举服务
/// </summary>
public class EnumService : ITransient
{
    /// <summary>
    /// 获取枚举描述值
    /// </summary>
    /// <returns></returns>
    public string GetDescription<T>(T enumValue) where T : Enum
    {
        var fields = enumValue.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            if (field.GetValue(null).Equals(enumValue))
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
        }
        return string.Empty;
    }
}
