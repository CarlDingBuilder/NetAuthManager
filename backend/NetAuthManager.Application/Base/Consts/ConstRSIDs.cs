using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Consts;

/// <summary>
/// RSID 配置
/// </summary>
public struct ConstRSIDs
{
    /// <summary>
    /// 基础模块
    /// </summary>
    public struct Base
    {
        ///// <summary>
        ///// 业务模块
        ///// </summary>
        //public const string BizMoudle = "7e54fcef-1060-4847-b11d-768bbdc79a6c";

        /// <summary>
        /// 菜单根
        /// </summary>
        public const string MenuRoot = "281073FE-60C6-43D3-BA75-9EA3D151020A";

        /// <summary>
        /// 资源根
        /// </summary>
        public const string ResourceRoot = "49AF1F87-7348-4D7A-ABEC-BD080261E4C6";
    }

    /// <summary>
    /// 角色 SID
    /// </summary>
    public struct RoleSID
    {
        /// <summary>
        /// Administrators
        /// </summary>
        public const string Administrators = "B639EB43-67D7-42fb-BD2E-B754BB11915B";

        /// <summary>
        /// Everyone
        /// </summary>
        public const string Everyone = "90674E5E-AC3C-4032-9EDF-7477F2247542";
    }
}
