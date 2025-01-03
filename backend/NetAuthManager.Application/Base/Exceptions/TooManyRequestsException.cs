using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Base.Exceptions;

/// <summary>
/// 过多请求错误
/// </summary>
public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message) : base(message) { }
}
