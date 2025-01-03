using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Models;

public enum LinqSelectOperator
{
    Contains,
    Equal,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,
    NotEqual,
    InWithEqual,  //对于多个值执行等于比较
    InWithContains,//对于多个值执行包含比较
    Between,
}
