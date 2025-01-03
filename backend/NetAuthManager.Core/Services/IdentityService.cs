using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Services;

/// <summary>
/// 增长值帮助器，如：SN
/// </summary>
public class IdentityService : BaseService<SysSequence>, ITransient
{
    #region 构造与注入

    public IdentityService(IRepository<SysSequence> sequenceRepository, IServiceScopeFactory scopeFactory) : base(sequenceRepository, scopeFactory)
    {
    }

    #endregion 构造与注入

    public static readonly object _locker = new object();

    /// <summary>
    /// 获取增长值，如生成：G90001，sysPrefix = bpm_transfar_，prefix = G9，length = 5
    /// </summary>
    /// <param name="prefix">增长唯一标识头，业务部分，数据库中需要使用</param>
    /// <param name="length">编码不包含头部的长度</param>
    /// <param name="sysPrefix">增长唯一标识头，固定部分，数据库中需要使用</param>
    /// <param name="containPrefix">生成的编码是否包含增长唯一标识头，业务部分</param>
    /// <param name="initial">初始值</param>
    /// <param name="increment">增长值</param>
    /// <returns>返回不包含头部的编码</returns>
    public string GetIdentityNo(string prefix, int length = 5, string sysPrefix = "sop_", bool containPrefix = true, int initial = 1, int increment = 1)
    {
        var actualPrefix = $"{sysPrefix}{prefix}";
        return new MonitorHelper($"GetIdentityNo_{actualPrefix}").TryDoReturn(() =>
        {
            //事务执行
            var newValue = TryTransDoReturn((dbContext) =>
            {
                var csequence = _entityRepository.FirstOrDefault(x => x.Prefix == actualPrefix);
                if (csequence == null)
                {
                    var sequenceItem = _entityRepository.Insert(new SysSequence
                    {
                        Prefix = actualPrefix,
                        CurValue = initial + increment,
                        ActiveDate = DateTime.Now,
                    });
                    return initial;
                }
                else
                {
                    var curValue = csequence.CurValue;
                    csequence.CurValue += increment;
                    _entityRepository.Update(csequence);
                    return curValue;
                }
            });
            if (!containPrefix) return $"{newValue.ToString().PadLeft(length, '0')}";
            return $"{prefix}{newValue.ToString().PadLeft(length, '0')}";

            //var connectionConfig = ConfigHelper.GetConnectionConfig();
            //using (OracleConnection oracleConnection = new OracleConnection(connectionConfig.ConnectionString + ";Enlist=false"))
            //{
            //    oracleConnection.Open();

            //    using (OracleCommand oracleCommand = new OracleCommand())
            //    {
            //        oracleCommand.Connection = oracleConnection;
            //        oracleCommand.BindByName = true;
            //        oracleCommand.CommandText = "insert into BPMSysSequence(Prefix, CurValue, ActiveDate) select :PREFIX,:INITIAL1,:ACTIVEDATE from dual where not exists(select 1 from BPMSysSequence where Prefix = :PREFIX)";
            //        oracleCommand.Parameters.Add(":PREFIX", OracleDbType.NVarchar2).Value = prefix;
            //        oracleCommand.Parameters.Add(":INITIAL1", OracleDbType.Int32).Value = initial;
            //        oracleCommand.Parameters.Add(":ACTIVEDATE", OracleDbType.Date).Value = DateTime.Now;
            //        ExecuteNonQuery(oracleCommand);
            //    }
            //    using (OracleCommand oracleCommand2 = new OracleCommand())
            //    {
            //        oracleCommand2.Connection = oracleConnection;
            //        oracleCommand2.BindByName = true;
            //        oracleCommand2.CommandText = "\r\nbegin\r\nselect CurValue into :SEEK from BPMSysSequence WHERE Prefix=:PREFIX for update;\r\nupdate BPMSysSequence set CurValue = CurValue+:INCREMENT WHERE Prefix=:PREFIX;\r\nend;";
            //        oracleCommand2.Parameters.Add(":PREFIX", OracleDbType.NVarchar2).Value = prefix;
            //        oracleCommand2.Parameters.Add(":INCREMENT", OracleDbType.Int32).Value = increment;
            //        oracleCommand2.Parameters.Add(":SEEK", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
            //        ExecuteNonQuery(oracleCommand2);
            //        rv = prefix + ((OracleDecimal)oracleCommand2.Parameters[":SEEK"].Value).ToInt32().ToString(new string('0', columns)); ;
            //    }
            //}
        });
    }
}
