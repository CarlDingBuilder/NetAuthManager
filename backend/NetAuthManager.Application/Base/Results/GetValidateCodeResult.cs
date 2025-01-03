using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Base.Results
{
    public class GetValidateCodeResult
    {
        /// <summary>
        /// 验证码图片Base64
        /// </summary>
        public string Base64String { get; set; }

        /// <summary>
        /// 存储验证码的KEy
        /// </summary>
        public string Keystore { get; set; }
    }
}
