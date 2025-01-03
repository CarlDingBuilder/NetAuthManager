using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models.DataSource;

public class YZDSFilterCollection : Dictionary<string, YZDSFilter>
{
    public YZDSFilterCollection() :
        base(StringComparer.OrdinalIgnoreCase)
    {
    }
}
