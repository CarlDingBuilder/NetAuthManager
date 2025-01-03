using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models.DataSource;

public class DataSourceRequest
{
    public int Start { get; set; } = 0;

    public int Limit { get; set; } = 10;

    public DataSourceIdentity ds { get; set; }

    public YZDSFilterCollection filters { get; set; }
}
