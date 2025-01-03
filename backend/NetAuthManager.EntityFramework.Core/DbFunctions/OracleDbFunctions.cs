using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.EntityFramework.Core.DbFunctions;

public static class OracleDbFunctions
{
    [DbFunction("DATEDIFF", IsBuiltIn = true)]
    public static int DateDiffInMinutes(DateTime startDate, DateTime endDate)
    {
        // 实际实现无需逻辑
        throw new NotSupportedException("This method must be used within a LINQ query.");
    }
}
