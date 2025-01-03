using NetAuthManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Resources;

/// <summary>
/// 资源项模型
/// </summary>
public class ResourceItemDto
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Label { get { return Name; } set { Name = value; } }

    /// <summary>
    /// SID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// SID类型
    /// </summary>
    public string SIDType { get; set; }

    /// <summary>
    /// 是否是根节点
    /// </summary>
    public bool IsRoot { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResourceItemDto() { }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResourceItemDto(SysMenu menu)
    {
        this.CopyFrom(menu);
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResourceItemDto(SysResourcePerm resourcePerm)
    {
        this.CopyFrom(resourcePerm);
    }

    protected virtual void CopyFrom(SysMenu menu)
    {
        if (menu == null) return;

        this.SID = menu.RSID;
        this.SIDType = Core.Common.Enums.SIDTypeEnum.MenuSID.ToString();
        this.Name = menu.Name;
    }

    protected virtual void CopyFrom(SysResourcePerm resourcePerm)
    {
        if (resourcePerm == null) return;

        this.SID = resourcePerm.SID;
        //this.SIDType = resourcePerm.SIDType.ToString();
        //this.Name = resourcePerm.SIDName;
    }
}

/// <summary>
/// 资源项包裹
/// </summary>
public class ResourceItemBoxDto
{
    /// <summary>
    /// 项目：菜单资源对应着全路径项集合
    /// </summary>
    public List<ResourceItemDto> Items { get; set; }

    /// <summary>
    /// 当前权限列表
    /// </summary>
    public List<ResourcePermItemDto> Perms { get; set; }

    /// <summary>
    /// 最大角色数量
    /// </summary>
    public int MaxRolesCount
    {
        get
        {
            if (Perms == null || Perms.Count == 0) return 0;
            return Perms.Max(p => (p.Roles == null ? 0 : p.Roles.Count));
        }
        private set { }
    }
}
