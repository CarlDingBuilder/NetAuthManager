using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Core.Views.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 字典服务
/// </summary>
public interface IDictionaryService
{
    /// <summary>
    /// 获取字典项列表
    /// </summary>
    Task<List<ViewSysDictionary>> GetList(string typeCode, string ext01 = null, string ext02 = null, string ext03 = null, string ext04 = null, string ext05 = null);

    /// <summary>
    /// 获取字典项
    /// </summary>
    Task<ViewSysDictionary> GetItem(string typeCode, string itemCode);

    /// <summary>
    /// 字典关联
    /// </summary>
    IQueryable<ViewSysDictionary> Join(string typeCode);

    /// <summary>
    /// 字典关联
    /// </summary>
    IQueryable<ViewSysDictionary> Join(string typeCode, string itemCode);
}
