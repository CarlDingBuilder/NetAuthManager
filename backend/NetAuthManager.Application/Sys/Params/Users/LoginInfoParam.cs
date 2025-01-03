using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Params.Users;

/// <summary>
/// 登录信息
/// </summary>
public class LoginInfoParam: GetTokenParam
{
    /// <summary>
    /// 图片验证码
    /// </summary>
    public string VerificationCode { get; set; }
    /// <summary>
    /// 存储私钥的key，用于解密 Password，解密后为 LoginInfoParam 对象
    /// </summary>
    public string Keystore { get; set; }
    /// <summary>
    /// 存储验证码的key，用来解密验证码
    /// </summary>
    public string VerifyCodeKeystore { get; set; }
}

/// <summary>
/// 获取Token参数
/// </summary>
public class GetTokenParam
{
    /// <summary>
    /// 用户账号
    /// </summary>
    /// <example>sa</example>
    public string UserName { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    /// <example>12345678</example>
    public string Password { get; set; }
}