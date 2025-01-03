using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NetAuthManager.Application.Sys.Results.RSA;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;

namespace NetAuthManager.Application;

/// <summary>
/// RSA服务
/// </summary>
public class RSAService: IRSAService, ITransient
{
    #region 注入与构造

    private readonly ICacheService _cacheService;
    public RSAService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    #endregion 注入与构造

    /// <summary>
    /// 获取公钥，并返回存储私钥的KEY
    /// </summary>
    public GetPublicKeyResult GetPublicKey()
    {
        // 生成 RSA 密钥对
        var rsaKeyPair = GenerateRsaKeyPair(1024);

        // 导出公钥和私钥
        string publicKey = ExportPublicKey(rsaKeyPair.Public);
        string privateKey = ExportPrivateKey(rsaKeyPair.Private);

        var keystore = Guid.NewGuid().ToString();
        _cacheService.Add(keystore, privateKey);
        return new GetPublicKeyResult
        {
            Keystore = keystore,
            PublicKey = publicKey
        };
    }

    /// <summary>
    /// 解密
    /// </summary>
    public string Decrypt(string encryptedData, string privateKey)
    {
        Core.Common.Helper.RSA rsa = new Core.Common.Helper.RSA(privateKey, noop: true);
        return rsa.DecodeOrNull(encryptedData);
    }

    private static AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize)
    {
        var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), keySize);
        var keyPairGenerator = new RsaKeyPairGenerator();
        keyPairGenerator.Init(keyGenerationParameters);
        return keyPairGenerator.GenerateKeyPair();
    }

    private static string ExportPublicKey(AsymmetricKeyParameter publicKey)
    {
        using (var stringWriter = new StringWriter())
        {
            var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(publicKey);
            pemWriter.Writer.Flush();
            return stringWriter.ToString();
        }
    }

    private static string ExportPrivateKey(AsymmetricKeyParameter privateKey)
    {
        using (var stringWriter = new StringWriter())
        {
            var pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(privateKey);
            pemWriter.Writer.Flush();
            return stringWriter.ToString();
        }
    }
}
