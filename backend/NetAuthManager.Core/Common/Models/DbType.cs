using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models;

public enum DbType
{
    MySql = 0,
    SqlServer = 1,
    Sqlite = 2,
    Oracle = 3,
    PostgreSQL = 4,
    Dm = 5,
    Kdbndp = 6,
    Oscar = 7,
    MySqlConnector = 8,
    Access = 9,
    OpenGauss = 10,
    QuestDB = 11,
    HG = 12,
    ClickHouse = 13,
    GBase = 14,
    Odbc = 0xF,
    OceanBaseForOracle = 0x10,
    TDengine = 17,
    GaussDB = 18,
    OceanBase = 19,
    Tidb = 20,
    Custom = 900
}
