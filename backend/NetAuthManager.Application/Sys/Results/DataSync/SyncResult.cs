using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.DataSync
{
    /// <summary>
    /// 同步结果
    /// </summary>
    public class SyncResult<T> : SyncResult
    {
        public T Data { get; set; }
        public SyncResult(string uuid) : base(uuid)
        {
        }
    }

    /// <summary>
    /// 同步结果
    /// </summary>
    public class SyncResult
    {
        public string uuid { get; set; }

        public SyncResult(string uuid)
        {
            this.uuid = uuid;
        }
    }
}
