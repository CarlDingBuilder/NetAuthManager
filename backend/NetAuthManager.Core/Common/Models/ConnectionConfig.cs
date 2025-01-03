using NetAuthManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models;

public class ConnectionConfig
{
    public dynamic ConfigId { get; set; }

    public DbType DbType { get; set; }

    public string DBVersion { get; set; }

    public string ConnectionString { get; set; }

    public string DbLinkName { get; set; }

    public bool IsAutoCloseConnection { get; set; }
}
