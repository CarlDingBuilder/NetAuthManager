using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models.DataSource;

public enum DSType
{
    UnKnow = 0,
    Table = 1,
    Procedure = 2,
    ESB = 3
}

public class DataSourceIdentity
{
    public DSType DSType
    {
        get
        {
            if (!String.IsNullOrEmpty(this.TableName))
                return DSType.Table;
            if (!String.IsNullOrEmpty(this.ProcedureName))
                return DSType.Procedure;
            if (!String.IsNullOrEmpty(this.ESB))
                return DSType.ESB;

            return DSType.UnKnow;
        }
    }
    public string DataSource { get; set; }
    public string TableName { get; set; }
    public string ProcedureName { get; set; }
    public string ESB { get; set; }
    public string OrderBy { get; set; }
}
