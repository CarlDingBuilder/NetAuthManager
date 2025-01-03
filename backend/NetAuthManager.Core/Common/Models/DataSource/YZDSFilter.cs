using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models.DataSource;

public class YZDSFilter
{
    public object value { get; set; }
    public bool afterBind { get; set; }
    public string op { get; set; }
}
