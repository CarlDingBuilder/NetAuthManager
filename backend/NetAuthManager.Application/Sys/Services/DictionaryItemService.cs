using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Application.Sys.Params.Dictionarys;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application;

/// <summary>
/// 字典项服务
/// </summary>
public class DictionaryItemService : BaseService<SysDictionaryItem>, IDictionaryItemService, ITransient
{
    #region 构造与注入

    private readonly IUserLoginService _userLoginService;
    private readonly IMenuResourceService _menuResourceService;
    private readonly IRepository<SysDictionaryType> _dictionaryTypeRepository;

    public DictionaryItemService(IRepository<SysDictionaryItem> entityRepository, IServiceScopeFactory scopeFactory, IUserLoginService userLoginService,
        IMenuResourceService menuResourceService, IRepository<SysDictionaryType> dictionaryTypeRepository) : base(entityRepository, scopeFactory)
    {
        _userLoginService = userLoginService;
        _menuResourceService = menuResourceService;
        _dictionaryTypeRepository = dictionaryTypeRepository;
    }

    #endregion 构造与注入

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// 公开访问，无需权限
    /// </summary>
    public override async Task<PageResult<SysDictionaryItem>> GetPageList(PageSearchParam param)
    {
        //字段类型
        if (param.filters == null || !param.filters.Any(x => x.Field == "TypeId"))
        {
            return new PageResult<SysDictionaryItem>();
        }

        //获取权限
        var perms = await _menuResourceService.GetResourceBizAllowPermNames();

        //权限构建
        var permExpression = ExpressionParserEx<SysDictionaryItem>.ParserConditionsByPerms(perms, "dicItem");

        //查询条件
        var whereExpression = ExpressionParser<SysDictionaryItem>.ParserConditions(param.filters, "dicItem");

        //分页查询
        var list = await _entityRepository.Where(permExpression)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<SysDictionaryItem>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 添加
    /// </summary>
    public async Task Add(DictionaryItemAddParam param)
    {
        if (string.IsNullOrEmpty(param.ItemCode))
            throw new Exception("字典项编码不能为空！");

        if (string.IsNullOrEmpty(param.ItemName))
            throw new Exception("字典项名称不能为空！");

        if (string.IsNullOrEmpty(param.TypeId))
            throw new Exception("字典类型ID不能为空！");

        if (!await _dictionaryTypeRepository.AnyAsync(x => x.Id == param.TypeId))
            throw new Exception("字典类型ID不能为空！");

        if (await AnyAsync(x => x.ItemCode == param.ItemCode && x.TypeId == param.TypeId))
            throw new Exception($"字典项 “{param.ItemCode}” 已存在，不能重复新增！");

        if (await AnyAsync(x => x.ItemName == param.ItemName && x.TypeId == param.TypeId))
            throw new Exception($"字典项 “{param.ItemName}” 已存在，不能重复新增！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //获取最大排序
        var orderIndex = await MaxAsync(x => true, x => x.OrderIndex);

        //新增
        var model = new SysDictionaryItem()
        {
            TypeId = param.TypeId,
            ItemCode = param.ItemCode,
            ItemName = param.ItemName,
            Ext01 = param.Ext01,
            Ext02 = param.Ext02,
            Ext03 = param.Ext03,
            Ext04 = param.Ext04,
            Ext05 = param.Ext05,
            OrderIndex = ++orderIndex,
            IsOpen = param.IsOpen,
            Description = param.Description,
            CreatorAccount = loginUser.UserAccount,
            CreatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName,
            CreatedTime = DateTime.Now,
        };

        //新增用户
        await _entityRepository.InsertNowAsync(model);
    }

    /// <summary>
    /// 修改
    /// </summary>
    public async Task Modify(DictionaryItemModifyParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典项ID不能为空！");

        if (string.IsNullOrEmpty(param.ItemCode))
            throw new Exception("字典项编码不能为空！");

        if (string.IsNullOrEmpty(param.ItemName))
            throw new Exception("字典项名称不能为空！");

        //历史数据
        var model = await GetByIdAsync(param.Id);
        if (model == null)
            throw new Exception($"当前修改的字典项不存在！");

        if (await AnyAsync(x => x.ItemCode == param.ItemCode && x.TypeId == model.TypeId && x.Id != model.Id))
            throw new Exception($"字典项 “{param.ItemCode}” 已存在，不能重复新增！");

        if (await AnyAsync(x => x.ItemName == param.ItemName && x.TypeId == model.TypeId && x.Id != model.Id))
            throw new Exception($"字典项 “{param.ItemName}” 已存在，不能重复新增！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //更新
        model.ItemCode = param.ItemCode;
        model.ItemName = param.ItemName;
        model.Ext01 = param.Ext01;
        model.Ext02 = param.Ext02;
        model.Ext03 = param.Ext03;
        model.Ext04 = param.Ext04;
        model.Ext05 = param.Ext05;
        model.OrderIndex = param.OrderIndex;
        model.IsOpen = param.IsOpen;
        model.Description = param.Description;
        model.UpdatorAccount = loginUser.UserAccount;
        model.UpdatorName = string.IsNullOrEmpty(loginUser.UserName) ? loginUser.UserAccount : loginUser.UserName;
        model.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(model);
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task Delete(DictionaryItemBaseParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典项ID不能为空！");

        //历史数据
        var model = await GetByIdAsync(param.Id);
        if (model != null)
        {
            //删除字典项
            await _entityRepository.DeleteNowAsync(model);
        }
    }

    /// <summary>
    /// 删除多个字典项
    /// </summary>
    public async Task Deletes(DictionaryItemDeletesParam param)
    {
        if (param.Ids == null || param.Ids.Count == 0)
            throw new Exception("字典项ID不能为空！");

        //历史数据
        var models = (from entity in _entityRepository.Entities
                      where param.Ids.Contains(entity.Id)
                      select entity).ToList();
        if (models != null && models.Count > 0)
        {
            //删除字典项
            await _entityRepository.DeleteNowAsync(models);
        }
    }

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    public async Task SetIsOpen(SetDictionaryItemIsOpenParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典项ID不能为空！");

        //数据
        var model = await GetByIdAsync(param.Id);
        if (model == null)
            throw new Exception($"当前修改的字典项不存在！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //执行更新
        model.IsOpen = param.IsOpen;
        model.UpdatorAccount = loginUser.UserAccount;
        model.UpdatorName = loginUser.UserName;
        model.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(model);
    }

    /// <summary>
    /// 排序
    /// </summary>
    public void Order(DictionaryItemOrderParam param)
    {
        if (param.Items != null && param.Items.Count > 0)
        {
            TryTransDo((dbContext) =>
            {
                foreach (var item in param.Items)
                {
                    // 创建一个新的实体实例
                    var entityToUpdate = new SysDictionaryItem
                    {
                        Id = item.Id, // 设置实体的主键值
                    };

                    // 附加实体到 DbContext
                    dbContext.Attach(entityToUpdate);

                    // 只标记特定的属性为已更改
                    entityToUpdate.OrderIndex = item.OrderIndex;
                    dbContext.Entry(entityToUpdate).Property(e => e.OrderIndex).IsModified = true;

                    dbContext.SaveChanges();
                }
            });
        }
    }
}
