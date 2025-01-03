using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetAuthManager.Application.Results;

/// <summary>
/// 基础结果
/// </summary>
[XmlRoot("XROOT")]
//[XmlInclude(typeof(BaseResult<SaveCustomersResult>))]
//[XmlInclude(typeof(BaseResult<SaveSuppliersResult>))]
public class BaseResult<T> : BaseResult
{
    /// <summary>
    /// 数据
    /// </summary>
    [XmlElement("DATA")]
    public T Data { get; set; }
}

/// <summary>
/// 基础结果
/// </summary>
[XmlRoot("XROOT")]
public class BaseResult
{
    public BaseResult() { }

    public BaseResult(bool success, string message) 
    {
        this.Success = success;
        this.Msg = message;
    }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    [XmlElement("SUCCESS")]
    public bool Success { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    [XmlElement("CODE")]
    public int Code { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    [XmlElement("MSG")]
    public string Msg
    {
        get
        {
            if (!string.IsNullOrEmpty(_msg)) return _msg;
            if (Errors != null)
            {
                if (Errors.GetType().Equals(typeof(string)))
                {
                    return Errors.ToString();
                }
            }
            return string.Empty;
        }
        set
        {
            _msg = value;
        }
    }
    private string _msg = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    [XmlElement("ERRORS")]
    [XmlIgnore]
    public object Errors { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    [XmlElement("EXTRAS")]
    [XmlIgnore]
    public object Extras { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [XmlElement("TIMESTAMP")]
    [XmlIgnore]
    public long Timestamp { get; set; }
}