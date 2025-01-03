using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace NetAuthManager.Application.Common.Helpers
{
    public class MD5Helper
    {
        public static string GetMd5(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(content);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
