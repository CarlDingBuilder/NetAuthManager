using NetAuthManager.Application.Base.Results;
using NetAuthManager.Application.Sys.Results.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

public interface IValidateCodeService
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    GetValidateCodeResult GetValidateCode();
}
