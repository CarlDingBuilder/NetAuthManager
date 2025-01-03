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
/// 字典服务
/// </summary>
public interface IDictionaryTypeService
{
    /// <summary>
    /// 获取字典分页列表
    /// </summary>
    Task<PageResult<SysDictionaryType>> GetPageList(PageSearchParam param);

    /// <summary>
    /// 添加
    /// </summary>
    Task Add(DictionaryTypeAddParam param);

    /// <summary>
    /// 修改
    /// </summary>
    Task Modify(DictionaryTypeModifyParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task Delete(DictionaryTypeBaseParam param);

    /// <summary>
    /// 删除多个字典项
    /// </summary>
    Task Deletes(DictionaryTypeDeletesParam param);

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    Task SetIsOpen(SetDictionaryTypeIsOpenParam param);
}
