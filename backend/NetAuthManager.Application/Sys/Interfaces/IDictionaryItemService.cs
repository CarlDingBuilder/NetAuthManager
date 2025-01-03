using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Application.Sys.Params.Dictionarys;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 字典项服务
/// </summary>
public interface IDictionaryItemService
{
    /// <summary>
    /// 获取字典项分页列表
    /// </summary>
    Task<PageResult<SysDictionaryItem>> GetPageList(PageSearchParam param);

    /// <summary>
    /// 添加
    /// </summary>
    Task Add(DictionaryItemAddParam param);

    /// <summary>
    /// 修改
    /// </summary>
    Task Modify(DictionaryItemModifyParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task Delete(DictionaryItemBaseParam param);

    /// <summary>
    /// 删除多个字典项
    /// </summary>
    Task Deletes(DictionaryItemDeletesParam param);

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    Task SetIsOpen(SetDictionaryItemIsOpenParam param);

    /// <summary>
    /// 排序
    /// </summary>
    void Order(DictionaryItemOrderParam param);
}
