using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Helper;

/// <summary>
/// 雪花ID生成
/// </summary>
public class SnowflakeIdGenerator : ISingleton
{
    private const long Epoch = 1577836800000; // 设置一个纪元时间（epoch），例如2020-01-01T00:00:00Z的时间戳
    private const int MachineIdBits = 5; // 机器ID所占位数
    private const int DatacenterIdBits = 5; // 数据中心ID所占位数
    private const int SequenceBits = 12; // 序列号占用的位数

    private const long MaxMachineId = -1L ^ (-1L << MachineIdBits); // 最大机器ID
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits); // 最大数据中心ID

    private const int MachineIdShift = SequenceBits; // 机器ID的偏移量（位移）
    private const int DatacenterIdShift = SequenceBits + MachineIdBits; // 数据中心ID的偏移量
    private const int TimestampLeftShift = SequenceBits + MachineIdBits + DatacenterIdBits; // 时间戳的偏移量

    private const long SequenceMask = -1L ^ (-1L << SequenceBits); // 序列号的掩码

    private long _machineId; // 机器ID
    private long _datacenterId; // 数据中心ID
    private long _sequence = 0L; // 序列号
    private long _lastTimestamp = -1L; // 上次生成ID的时间戳

    /// <summary>
    /// 构造函数
    /// </summary>
    public SnowflakeIdGenerator() : this(1, 1) { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="datacenterId">数据中心ID，用于标识数据中心的，可以是 1、2、3 等</param>
    /// <param name="machineId">机器ID，用于标识机器，可以是 1、2、3 等</param>
    public SnowflakeIdGenerator(long datacenterId, long machineId)
    {
        if (machineId > MaxMachineId || machineId < 0)
        {
            throw new ArgumentException($"Machine ID can't be greater than {MaxMachineId} or less than 0");
        }

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentException($"Datacenter ID can't be greater than {MaxDatacenterId} or less than 0");
        }

        _machineId = machineId;
        _datacenterId = datacenterId;
    }

    /// <summary>
    /// 获取下一个ID
    /// </summary>
    public long NextId()
    {
        lock (this)
        {
            var timestamp = TimeGen();

            if (timestamp < _lastTimestamp)
            {
                throw new Exception("Clock moved backwards. Refusing to generate id for " + (_lastTimestamp - timestamp) + " milliseconds");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;
            return ((timestamp - Epoch) << TimestampLeftShift) | (_datacenterId << DatacenterIdShift) | (_machineId << MachineIdShift) | _sequence;
        }
    }

    private long TilNextMillis(long lastTimestamp)
    {
        var timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }
        return timestamp;
    }

    private static long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
