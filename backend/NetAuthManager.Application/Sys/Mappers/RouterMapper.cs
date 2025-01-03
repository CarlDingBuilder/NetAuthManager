using NetAuthManager.Core.Common.Extended;
using NetAuthManager.Core.Entities;
using Newtonsoft.Json;
using System.Collections;

namespace NetAuthManager.Application.Sys.Mappers;

/// <summary>
/// 路由构建 Mapper
/// </summary>
public class RouterMapper : IRegister, ITransient
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<SysMenu, Hashtable>()
        .Map(dest => dest, src => BuildRouter(src));
    }

    /// <summary>
    /// 路由构建
    /// </summary>
    protected List<Hashtable> BuildRouter(List<SysMenu> src)
    {
        var list = new List<Hashtable>();
        foreach (var menu in src)
        {
            if (!menu.IsMenu) continue;
            list.Add(BuildRouter(menu));
        }
        return list;
    }

    /// <summary>
    /// 路由构建
    /// </summary>
    protected Hashtable BuildRouter(SysMenu menu)
    {
        var model = new Hashtable();

        //当前菜单
        menu.FormatToHashtable(model);

        //子菜单
        if (menu.Children != null) model["children"] = BuildRouter(menu.Children);

        return model;
    }
}
