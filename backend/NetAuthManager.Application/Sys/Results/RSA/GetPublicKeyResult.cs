using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.RSA;

public class GetPublicKeyResult
{
    public string Keystore { get; set; }
    public string PublicKey { get; set; }
}
