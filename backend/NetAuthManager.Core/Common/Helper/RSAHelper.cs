using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Numerics;

namespace NetAuthManager.Core.Common.Helper;

public class RSA
{
    private RSACryptoServiceProvider rsa;

    public RSACryptoServiceProvider RSAObject => rsa;

    public int KeySize => rsa.KeySize;

    public bool HasPrivate => !rsa.PublicOnly;

    public string ToXML(bool convertToPublic = false)
    {
        return rsa.ToXmlString(!rsa.PublicOnly && !convertToPublic);
    }

    public RSA_PEM ToPEM(bool convertToPublic = false)
    {
        return new RSA_PEM(rsa, convertToPublic);
    }

    public string Encode(string str)
    {
        return Convert.ToBase64String(Encode(Encoding.UTF8.GetBytes(str)));
    }

    public byte[] Encode(byte[] data)
    {
        int blockLen = rsa.KeySize / 8 - 11;
        if (data.Length <= blockLen)
        {
            return rsa.Encrypt(data, fOAEP: false);
        }
        using MemoryStream dataStream = new MemoryStream(data);
        using MemoryStream enStream = new MemoryStream();
        byte[] buffer = new byte[blockLen];
        for (int len = dataStream.Read(buffer, 0, blockLen); len > 0; len = dataStream.Read(buffer, 0, blockLen))
        {
            byte[] block = new byte[len];
            Array.Copy(buffer, 0, block, 0, len);
            byte[] enBlock = rsa.Encrypt(block, fOAEP: false);
            enStream.Write(enBlock, 0, enBlock.Length);
        }
        return enStream.ToArray();
    }

    public string DecodeOrNull(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        byte[] byts = null;
        try
        {
            byts = Convert.FromBase64String(str);
        }
        catch
        {
        }
        if (byts == null)
        {
            return null;
        }
        byte[] val = DecodeOrNull(byts);
        if (val == null)
        {
            return null;
        }
        return Encoding.UTF8.GetString(val);
    }

    public byte[] DecodeOrNull(byte[] data)
    {
        try
        {
            int blockLen = rsa.KeySize / 8;
            if (data.Length <= blockLen)
            {
                return rsa.Decrypt(data, fOAEP: false);
            }
            using MemoryStream dataStream = new MemoryStream(data);
            using MemoryStream deStream = new MemoryStream();
            byte[] buffer = new byte[blockLen];
            for (int len = dataStream.Read(buffer, 0, blockLen); len > 0; len = dataStream.Read(buffer, 0, blockLen))
            {
                byte[] block = new byte[len];
                Array.Copy(buffer, 0, block, 0, len);
                byte[] deBlock = rsa.Decrypt(block, fOAEP: false);
                deStream.Write(deBlock, 0, deBlock.Length);
            }
            return deStream.ToArray();
        }
        catch
        {
            return null;
        }
    }

    public string Sign(string hash, string str)
    {
        return Convert.ToBase64String(Sign(hash, Encoding.UTF8.GetBytes(str)));
    }

    public byte[] Sign(string hash, byte[] data)
    {
        return rsa.SignData(data, hash);
    }

    public bool Verify(string hash, string sgin, string str)
    {
        byte[] byts = null;
        try
        {
            byts = Convert.FromBase64String(sgin);
        }
        catch
        {
        }
        if (byts == null)
        {
            return false;
        }
        return Verify(hash, byts, Encoding.UTF8.GetBytes(str));
    }

    public bool Verify(string hash, byte[] sgin, byte[] data)
    {
        try
        {
            return rsa.VerifyData(data, hash, sgin);
        }
        catch
        {
            return false;
        }
    }

    public RSA(int keySize)
    {
        rsa = new RSACryptoServiceProvider(keySize, new CspParameters
        {
            Flags = CspProviderFlags.UseMachineKeyStore
        });
    }

    public RSA(string xml)
    {
        rsa = new RSACryptoServiceProvider(new CspParameters
        {
            Flags = CspProviderFlags.UseMachineKeyStore
        });
        rsa.FromXmlString(xml);
    }

    public RSA(string pem, bool noop)
    {
        rsa = RSA_PEM.FromPEM(pem).GetRSA();
    }

    public RSA(RSA_PEM pem)
    {
        rsa = pem.GetRSA();
    }

    public RSA(byte[] modulus, byte[] exponent, byte[] dOrNull)
    {
        rsa = new RSA_PEM(modulus, exponent, dOrNull).GetRSA();
    }

    public RSA(byte[] modulus, byte[] exponent, byte[] d, byte[] p, byte[] q, byte[] dp, byte[] dq, byte[] inverseQ)
    {
        rsa = new RSA_PEM(modulus, exponent, d, p, q, dp, dq, inverseQ).GetRSA();
    }
}

public class RSA_PEM
{
    public byte[] Key_Modulus;

    public byte[] Key_Exponent;

    public byte[] Key_D;

    public byte[] Val_P;

    public byte[] Val_Q;

    public byte[] Val_DP;

    public byte[] Val_DQ;

    public byte[] Val_InverseQ;

    private static readonly Regex _PEMCode = new Regex("--+.+?--+|\\s+");

    private static readonly byte[] _SeqOID = new byte[15]
    {
        48, 13, 6, 9, 42, 134, 72, 134, 247, 13,
        1, 1, 1, 5, 0
    };

    private static readonly byte[] _Ver = new byte[3] { 2, 1, 0 };

    private static readonly Regex xmlExp = new Regex("\\s*<RSAKeyValue>([<>\\/\\+=\\w\\s]+)</RSAKeyValue>\\s*");

    private static readonly Regex xmlTagExp = new Regex("<(.+?)>\\s*([^<]+?)\\s*</");

    public int KeySize => Key_Modulus.Length * 8;

    public bool HasPrivate => Key_D != null;

    private RSA_PEM()
    {
    }

    public RSA_PEM(RSACryptoServiceProvider rsa, bool convertToPublic = false)
    {
        bool isPublic = convertToPublic || rsa.PublicOnly;
        RSAParameters param = rsa.ExportParameters(!isPublic);
        Key_Modulus = param.Modulus;
        Key_Exponent = param.Exponent;
        if (!isPublic)
        {
            Key_D = param.D;
            Val_P = param.P;
            Val_Q = param.Q;
            Val_DP = param.DP;
            Val_DQ = param.DQ;
            Val_InverseQ = param.InverseQ;
        }
    }

    public RSA_PEM(byte[] modulus, byte[] exponent, byte[] d, byte[] p, byte[] q, byte[] dp, byte[] dq, byte[] inverseQ)
    {
        Key_Modulus = modulus;
        Key_Exponent = exponent;
        Key_D = BigL(d, modulus.Length);
        int keyLen = modulus.Length / 2;
        Val_P = BigL(p, keyLen);
        Val_Q = BigL(q, keyLen);
        Val_DP = BigL(dp, keyLen);
        Val_DQ = BigL(dq, keyLen);
        Val_InverseQ = BigL(inverseQ, keyLen);
    }

    public RSA_PEM(byte[] modulus, byte[] exponent, byte[] dOrNull)
    {
        Key_Modulus = modulus;
        Key_Exponent = exponent;
        if (dOrNull != null)
        {
            Key_D = BigL(dOrNull, modulus.Length);
            BigInteger i = BigX(modulus);
            BigInteger e = BigX(exponent);
            BigInteger d = BigX(dOrNull);
            BigInteger p = FindFactor(e, d, i);
            BigInteger q = i / p;
            if (p.CompareTo(q) > 0)
            {
                BigInteger t = p;
                p = q;
                q = t;
            }
            BigInteger exp1 = d % (p - BigInteger.One);
            BigInteger exp2 = d % (q - BigInteger.One);
            BigInteger coeff = BigInteger.ModPow(q, p - 2, p);
            int keyLen = modulus.Length / 2;
            Val_P = BigL(BigB(p), keyLen);
            Val_Q = BigL(BigB(q), keyLen);
            Val_DP = BigL(BigB(exp1), keyLen);
            Val_DQ = BigL(BigB(exp2), keyLen);
            Val_InverseQ = BigL(BigB(coeff), keyLen);
        }
    }

    public RSACryptoServiceProvider GetRSA()
    {
        CspParameters rsaParams = new CspParameters();
        rsaParams.Flags = CspProviderFlags.UseMachineKeyStore;
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(rsaParams);
        RSAParameters param = default(RSAParameters);
        param.Modulus = Key_Modulus;
        param.Exponent = Key_Exponent;
        if (Key_D != null)
        {
            param.D = Key_D;
            param.P = Val_P;
            param.Q = Val_Q;
            param.DP = Val_DP;
            param.DQ = Val_DQ;
            param.InverseQ = Val_InverseQ;
        }
        rsa.ImportParameters(param);
        return rsa;
    }

    public static BigInteger BigX(byte[] bigb)
    {
        if (bigb[0] > 127)
        {
            byte[] c = new byte[bigb.Length + 1];
            Array.Copy(bigb, 0, c, 1, bigb.Length);
            bigb = c;
        }
        return new BigInteger(bigb.Reverse().ToArray());
    }

    public static byte[] BigB(BigInteger bigx)
    {
        byte[] val = bigx.ToByteArray().Reverse().ToArray();
        if (val[0] == 0)
        {
            byte[] c = new byte[val.Length - 1];
            Array.Copy(val, 1, c, 0, c.Length);
            val = c;
        }
        return val;
    }

    public static byte[] BigL(byte[] bytes, int keyLen)
    {
        if (keyLen - bytes.Length == 1)
        {
            byte[] c = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, c, 1, bytes.Length);
            bytes = c;
        }
        return bytes;
    }

    private static BigInteger FindFactor(BigInteger e, BigInteger d, BigInteger n)
    {
        BigInteger edMinus1 = e * d - BigInteger.One;
        int s = -1;
        if (edMinus1 != BigInteger.Zero)
        {
            s = (int)(BigInteger.Log(edMinus1 & -edMinus1) / BigInteger.Log(2));
        }
        BigInteger t = edMinus1 >> s;
        long now = DateTime.Now.Ticks;
        for (int aInt = 2; aInt % 10 != 0 || DateTime.Now.Ticks - now <= 30000000; aInt++)
        {
            BigInteger aPow = BigInteger.ModPow(new BigInteger(aInt), t, n);
            for (int i = 1; i <= s; i++)
            {
                if (aPow == BigInteger.One)
                {
                    break;
                }
                if (aPow == n - BigInteger.One)
                {
                    break;
                }
                BigInteger aPowSquared = aPow * aPow % n;
                if (aPowSquared == BigInteger.One)
                {
                    return BigInteger.GreatestCommonDivisor(aPow - BigInteger.One, n);
                }
                aPow = aPowSquared;
            }
        }
        throw new Exception("推算RSA.P超时");
    }

    public static RSA_PEM FromPEM(string pem)
    {
        RSA_PEM param = new RSA_PEM();
        string base64 = _PEMCode.Replace(pem, "");
        byte[] data = null;
        try
        {
            data = Convert.FromBase64String(base64);
        }
        catch
        {
        }
        if (data == null)
        {
            throw new Exception("PEM内容无效");
        }
        int idx = 0;
        Func<byte, int> readLen = delegate (byte first)
        {
            if (data[idx] == first)
            {
                idx++;
                if (data[idx] == 129)
                {
                    idx++;
                    return data[idx++];
                }
                if (data[idx] == 130)
                {
                    idx++;
                    return (data[idx++] << 8) + data[idx++];
                }
                if (data[idx] < 128)
                {
                    return data[idx++];
                }
            }
            throw new Exception("PEM未能提取到数据");
        };
        Func<byte[]> readBlock = delegate
        {
            int num2 = readLen(2);
            if (data[idx] == 0)
            {
                idx++;
                num2--;
            }
            byte[] array = new byte[num2];
            for (int i = 0; i < num2; i++)
            {
                array[i] = data[idx + i];
            }
            idx += num2;
            return array;
        };
        Func<byte[], bool> eq = delegate (byte[] byts)
        {
            int num = 0;
            while (num < byts.Length)
            {
                if (idx >= data.Length)
                {
                    return false;
                }
                if (byts[num] != data[idx])
                {
                    return false;
                }
                num++;
                idx++;
            }
            return true;
        };
        if (pem.Contains("PUBLIC KEY"))
        {
            readLen(48);
            int idx3 = idx;
            if (eq(_SeqOID))
            {
                readLen(3);
                idx++;
                readLen(48);
            }
            else
            {
                idx = idx3;
            }
            param.Key_Modulus = readBlock();
            param.Key_Exponent = readBlock();
        }
        else
        {
            if (!pem.Contains("PRIVATE KEY"))
            {
                throw new Exception("pem需要BEGIN END标头");
            }
            readLen(48);
            if (!eq(_Ver))
            {
                throw new Exception("PEM未知版本");
            }
            int idx2 = idx;
            if (eq(_SeqOID))
            {
                readLen(4);
                readLen(48);
                if (!eq(_Ver))
                {
                    throw new Exception("PEM版本无效");
                }
            }
            else
            {
                idx = idx2;
            }
            param.Key_Modulus = readBlock();
            param.Key_Exponent = readBlock();
            int keyLen = param.Key_Modulus.Length;
            param.Key_D = BigL(readBlock(), keyLen);
            keyLen /= 2;
            param.Val_P = BigL(readBlock(), keyLen);
            param.Val_Q = BigL(readBlock(), keyLen);
            param.Val_DP = BigL(readBlock(), keyLen);
            param.Val_DQ = BigL(readBlock(), keyLen);
            param.Val_InverseQ = BigL(readBlock(), keyLen);
        }
        return param;
    }

    public string ToPEM_PKCS1(bool convertToPublic = false)
    {
        return ToPEM(convertToPublic, privateUsePKCS8: false, publicUsePKCS8: false);
    }

    public string ToPEM_PKCS8(bool convertToPublic = false)
    {
        return ToPEM(convertToPublic, privateUsePKCS8: true, publicUsePKCS8: true);
    }

    public string ToPEM(bool convertToPublic, bool privateUsePKCS8, bool publicUsePKCS8)
    {
        MemoryStream ms = new MemoryStream();
        Action<int> writeLenByte = delegate (int len)
        {
            if (len < 128)
            {
                ms.WriteByte((byte)len);
            }
            else if (len <= 255)
            {
                ms.WriteByte(129);
                ms.WriteByte((byte)len);
            }
            else
            {
                ms.WriteByte(130);
                ms.WriteByte((byte)((uint)(len >> 8) & 0xFFu));
                ms.WriteByte((byte)((uint)len & 0xFFu));
            }
        };
        Action<byte[]> writeBlock = delegate (byte[] byts)
        {
            bool flag3 = byts[0] >> 4 >= 8;
            ms.WriteByte(2);
            int obj = byts.Length + (flag3 ? 1 : 0);
            writeLenByte(obj);
            if (flag3)
            {
                ms.WriteByte(0);
            }
            ms.Write(byts, 0, byts.Length);
        };
        Func<int, byte[], byte[]> writeLen = delegate (int index, byte[] byts)
        {
            int num = byts.Length - index;
            ms.SetLength(0L);
            ms.Write(byts, 0, index);
            writeLenByte(num);
            ms.Write(byts, index, num);
            return ms.ToArray();
        };
        Action<MemoryStream, byte[]> writeAll = delegate (MemoryStream stream, byte[] byts)
        {
            stream.Write(byts, 0, byts.Length);
        };
        Func<string, int, string> TextBreak = delegate (string text, int line)
        {
            int i = 0;
            int length = text.Length;
            StringBuilder stringBuilder = new StringBuilder();
            for (; i < length; i += line)
            {
                if (i > 0)
                {
                    stringBuilder.Append('\n');
                }
                if (i + line >= length)
                {
                    stringBuilder.Append(text.Substring(i));
                }
                else
                {
                    stringBuilder.Append(text.Substring(i, line));
                }
            }
            return stringBuilder.ToString();
        };
        if (Key_D == null || convertToPublic)
        {
            ms.WriteByte(48);
            int index3 = (int)ms.Length;
            int index5 = -1;
            int index7 = -1;
            if (publicUsePKCS8)
            {
                writeAll(ms, _SeqOID);
                ms.WriteByte(3);
                index5 = (int)ms.Length;
                ms.WriteByte(0);
                ms.WriteByte(48);
                index7 = (int)ms.Length;
            }
            writeBlock(Key_Modulus);
            writeBlock(Key_Exponent);
            byte[] byts3 = ms.ToArray();
            if (index5 != -1)
            {
                byts3 = writeLen(index7, byts3);
                byts3 = writeLen(index5, byts3);
            }
            byts3 = writeLen(index3, byts3);
            string flag2 = " PUBLIC KEY";
            if (!publicUsePKCS8)
            {
                flag2 = " RSA" + flag2;
            }
            return "-----BEGIN" + flag2 + "-----\n" + TextBreak(Convert.ToBase64String(byts3), 64) + "\n-----END" + flag2 + "-----";
        }
        ms.WriteByte(48);
        int index2 = (int)ms.Length;
        writeAll(ms, _Ver);
        int index4 = -1;
        int index6 = -1;
        if (privateUsePKCS8)
        {
            writeAll(ms, _SeqOID);
            ms.WriteByte(4);
            index4 = (int)ms.Length;
            ms.WriteByte(48);
            index6 = (int)ms.Length;
            writeAll(ms, _Ver);
        }
        writeBlock(Key_Modulus);
        writeBlock(Key_Exponent);
        writeBlock(Key_D);
        writeBlock(Val_P);
        writeBlock(Val_Q);
        writeBlock(Val_DP);
        writeBlock(Val_DQ);
        writeBlock(Val_InverseQ);
        byte[] byts2 = ms.ToArray();
        if (index4 != -1)
        {
            byts2 = writeLen(index6, byts2);
            byts2 = writeLen(index4, byts2);
        }
        byts2 = writeLen(index2, byts2);
        string flag = " PRIVATE KEY";
        if (!privateUsePKCS8)
        {
            flag = " RSA" + flag;
        }
        return "-----BEGIN" + flag + "-----\n" + TextBreak(Convert.ToBase64String(byts2), 64) + "\n-----END" + flag + "-----";
    }

    public static RSA_PEM FromXML(string xml)
    {
        RSA_PEM rtv = new RSA_PEM();
        Match xmlM = xmlExp.Match(xml);
        if (!xmlM.Success)
        {
            throw new Exception("XML内容不符合要求");
        }
        Match tagM = xmlTagExp.Match(xmlM.Groups[1].Value);
        while (tagM.Success)
        {
            string tag = tagM.Groups[1].Value;
            string b64 = tagM.Groups[2].Value;
            byte[] val = Convert.FromBase64String(b64);
            switch (tag)
            {
                case "Modulus":
                    rtv.Key_Modulus = val;
                    break;
                case "Exponent":
                    rtv.Key_Exponent = val;
                    break;
                case "D":
                    rtv.Key_D = val;
                    break;
                case "P":
                    rtv.Val_P = val;
                    break;
                case "Q":
                    rtv.Val_Q = val;
                    break;
                case "DP":
                    rtv.Val_DP = val;
                    break;
                case "DQ":
                    rtv.Val_DQ = val;
                    break;
                case "InverseQ":
                    rtv.Val_InverseQ = val;
                    break;
            }
            tagM = tagM.NextMatch();
        }
        if (rtv.Key_Modulus == null || rtv.Key_Exponent == null)
        {
            throw new Exception("XML公钥丢失");
        }
        if (rtv.Key_D != null && (rtv.Val_P == null || rtv.Val_Q == null || rtv.Val_DP == null || rtv.Val_DQ == null || rtv.Val_InverseQ == null))
        {
            return new RSA_PEM(rtv.Key_Modulus, rtv.Key_Exponent, rtv.Key_D);
        }
        return rtv;
    }

    public string ToXML(bool convertToPublic)
    {
        StringBuilder str = new StringBuilder();
        str.Append("<RSAKeyValue>");
        str.Append("<Modulus>" + Convert.ToBase64String(Key_Modulus) + "</Modulus>");
        str.Append("<Exponent>" + Convert.ToBase64String(Key_Exponent) + "</Exponent>");
        if (!(Key_D == null || convertToPublic))
        {
            str.Append("<P>" + Convert.ToBase64String(Val_P) + "</P>");
            str.Append("<Q>" + Convert.ToBase64String(Val_Q) + "</Q>");
            str.Append("<DP>" + Convert.ToBase64String(Val_DP) + "</DP>");
            str.Append("<DQ>" + Convert.ToBase64String(Val_DQ) + "</DQ>");
            str.Append("<InverseQ>" + Convert.ToBase64String(Val_InverseQ) + "</InverseQ>");
            str.Append("<D>" + Convert.ToBase64String(Key_D) + "</D>");
        }
        str.Append("</RSAKeyValue>");
        return str.ToString();
    }
}