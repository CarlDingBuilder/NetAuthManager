using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Consts;

/// <summary>
/// 名称常量
/// </summary>
public struct ConstNames
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public struct RoleName
    {
        /// <summary>
        /// Administrators
        /// </summary>
        public const string Administrators = "超级管理员";

        /// <summary>
        /// Everyone
        /// </summary>
        public const string Everyone = "所有人";
    }

    /// <summary>
    /// 角色编码
    /// </summary>
    public struct RoleCode
    {
        /// <summary>
        /// Administrators
        /// </summary>
        public const string Administrators = "Administrators";

        /// <summary>
        /// Everyone
        /// </summary>
        public const string Everyone = "Everyone";
    }

    /// <summary>
    /// 资源权限类型
    /// </summary>
    public struct ResourcePermTypeName
    {
        /// <summary>
        /// 操作
        /// </summary>
        public const string Module = "操作";

        /// <summary>
        /// 数据
        /// </summary>
        public const string Record = "数据";
    }

    /// <summary>
    /// 资源权限类型值
    /// </summary>
    public struct ResourcePermTypeValue
    {
        /// <summary>
        /// 系统
        /// </summary>
        public const string System = "System";

        /// <summary>
        /// 操作
        /// </summary>
        public const string Module = "Module";

        /// <summary>
        /// 数据
        /// </summary>
        public const string Record = "Record";
    }
}
