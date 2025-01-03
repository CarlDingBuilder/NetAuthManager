using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetAuthManager.Application.Sys.Params.Users;

/// <summary>
/// 用户基本参数
/// </summary>
public class UserBaseParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// HRID
    /// </summary>
    public string HRID { get; set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// 用户添加参数
/// </summary>
public class UserAddParam : UserBaseParam
{
}

/// <summary>
/// 用户修改参数
/// </summary>
public class UserModifyParam : UserBaseParam
{
}

/// <summary>
/// 用户修改密码参数
/// </summary>
public class UserModifyPasswordParam : UserBaseParam
{
    public string Password { get; set; }
    public string PasswordAgain { get; set; }
}

/// <summary>
/// 用户启用参数
/// </summary>
public class SetUserIsOpenParam : UserBaseParam
{
}

/// <summary>
/// 用户删除参数
/// </summary>
public class UserDeletesParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    public List<string> Accounts { get; set; }
}

/// <summary>
/// 保存角色成员
/// </summary>
public class SaveUserRolesParam : UserBaseParam
{
    /// <summary>
    /// 角色编码
    /// </summary>
    public List<string> RoleCodes { get; set; }
}