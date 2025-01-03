using NetAuthManager.Application.Sys.Results.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

public interface IRSAService
{
    /// <summary>
    /// 获取公钥，并返回存储私钥的KEY
    /// </summary>
    GetPublicKeyResult GetPublicKey();

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="encryptedData"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    string Decrypt(string encryptedData, string privateKey);
}
