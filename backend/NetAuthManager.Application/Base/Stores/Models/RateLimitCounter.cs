using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Stores
{
    /// <summary>
    /// 限流总数
    /// </summary>
    public class RateLimitCounter
    {
        public DateTime Timestamp { get; set; }
        public int TotalRequests { get; set; }
    }
}
