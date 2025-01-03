using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Exceptions;

/// <summary>
/// 自定义错误
/// </summary>
public class CustomException : Exception
{
    /// <summary>
    /// 是否需要记录日志，如果需要，被系统拦截到后会记录日志
    /// </summary>
    public bool ShouldLogError { get; set; }

    public CustomException(string errorMessage, bool shouldLogError = false) : base(errorMessage)
    {
        this.ShouldLogError = shouldLogError;
    }

    public CustomException(CustomException exception, bool shouldLogError = false) : base(exception.Message, exception.InnerException)
    {
        this.ShouldLogError = shouldLogError;
    }

    public CustomException(Exception exception, bool shouldLogError = false) : base(exception.Message, exception)
    {
        this.ShouldLogError = shouldLogError;
    }
}
