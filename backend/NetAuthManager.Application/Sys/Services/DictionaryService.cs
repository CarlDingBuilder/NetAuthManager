using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Application.Sys.Params.Dictionarys;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
using NetAuthManager.Core.Views.Sys;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 字典服务
/// </summary>
public class DictionaryService : IDictionaryService, ITransient
{
    #region 构造与注入

    private readonly IRepository<SysDictionaryType> _dictionaryTypeRepository;
    private readonly IRepository<SysDictionaryItem> _dictionaryItemRepository;

    public DictionaryService(IRepository<SysDictionaryType> dictionaryTypeRepository, IRepository<SysDictionaryItem> dictionaryItemRepository)
    {
        _dictionaryTypeRepository = dictionaryTypeRepository;
        _dictionaryItemRepository = dictionaryItemRepository;
    }

    #endregion 构造与注入

    /// <summary>
    /// 字典列表
    /// </summary>
    public async Task<List<ViewSysDictionary>> GetList(string typeCode, string ext01 = null, string ext02 = null, string ext03 = null, string ext04 = null, string ext05 = null)
    {
        var query = Join(typeCode);
        if (ext01 == null) { }
        else if (string.IsNullOrEmpty(ext01)) query = query.Where(x => string.IsNullOrEmpty(x.Ext01));
        else query = query.Where(x => x.Ext01 == ext01);
        if (ext02 == null) { }
        else if (string.IsNullOrEmpty(ext02)) query = query.Where(x => string.IsNullOrEmpty(x.Ext02));
        else query = query.Where(x => x.Ext02 == ext02);
        if (ext03 == null) { }
        else if (string.IsNullOrEmpty(ext03)) query = query.Where(x => string.IsNullOrEmpty(x.Ext03));
        else query = query.Where(x => x.Ext03 == ext03);
        if (ext04 == null) { }
        else if (string.IsNullOrEmpty(ext04)) query = query.Where(x => string.IsNullOrEmpty(x.Ext04));
        else query = query.Where(x => x.Ext04 == ext04);
        if (ext05 == null) { }
        else if (string.IsNullOrEmpty(ext05)) query = query.Where(x => string.IsNullOrEmpty(x.Ext05));
        else query = query.Where(x => x.Ext05 == ext05);

        var list = await query.ToListAsync();
        return list;
    }

    /// <summary>
    /// 获取字典项
    /// </summary>
    /// <param name="typeCode">字典编码</param>
    /// <param name="itemCode">字典项值</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ViewSysDictionary> GetItem(string typeCode, string itemCode)
    {
        return await Join(typeCode, itemCode).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 字典关联
    /// </summary>
    public IQueryable<ViewSysDictionary> Join(string typeCode)
    {
        return (from dicType in _dictionaryTypeRepository.Entities
                where dicType.TypeCode.Equals(typeCode)
                join dicItem in _dictionaryItemRepository.Entities on dicType.Id equals dicItem.TypeId
                orderby dicItem.OrderIndex
                select new ViewSysDictionary
                {
                    TypeCode = dicType.TypeCode,
                    TypeName = dicType.TypeName,
                    ItemCode = dicItem.ItemCode,
                    ItemName = dicItem.ItemName,
                    Ext01 = dicItem.Ext01,
                    Ext02 = dicItem.Ext02,
                    Ext03 = dicItem.Ext03,
                    Ext04 = dicItem.Ext04,
                    Ext05 = dicItem.Ext05,
                    OrderIndex = dicItem.OrderIndex,
                });
    }

    /// <summary>
    /// 字典关联
    /// </summary>
    public IQueryable<ViewSysDictionary> Join(string typeCode, string itemCode)
    {
        return (from dicType in _dictionaryTypeRepository.Entities
                where dicType.TypeCode.Equals(typeCode)
                join dicItem in _dictionaryItemRepository.Entities on dicType.Id equals dicItem.TypeId
                where dicItem.ItemCode == itemCode
                select new ViewSysDictionary
                {
                    TypeCode = dicType.TypeCode,
                    TypeName = dicType.TypeName,
                    ItemCode = dicItem.ItemCode,
                    ItemName = dicItem.ItemName,
                    Ext01 = dicItem.Ext01,
                    Ext02 = dicItem.Ext02,
                    Ext03 = dicItem.Ext03,
                    Ext04 = dicItem.Ext04,
                    Ext05 = dicItem.Ext05,
                    OrderIndex = dicItem.OrderIndex,
                });
    }
}
