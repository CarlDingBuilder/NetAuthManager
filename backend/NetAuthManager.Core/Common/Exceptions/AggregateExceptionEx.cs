using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Exceptions
{
    public static class AggregateExceptionEx
    {
        /// <summary>
        /// 获取自定义格式消息
        /// </summary>
        public static string GetMessage(this AggregateException exception)
        {
            if (exception == null) return string.Empty;
            else if (exception.InnerExceptions.Count == 0) return string.Empty;
            else if (exception.InnerExceptions.Count == 1) return exception.InnerException.Message;
            else
            {
                var message = string.Empty;
                var rowIndex = 0;
                foreach (var innerException in exception.InnerExceptions)
                {
                    message += $"错误{++rowIndex}：{innerException.Message}";
                }
                return message;
            }
        }
    }
}
