using BPM.FSSC.DBCore.Entity;
using NetAuthManager.Application.Sys.Params.Dictionarys;
using NetAuthManager.Application.Sys.Params.Users;
using NetAuthManager.Core.Entities;
using NetAuthManager.Core.Expressions;
using NetAuthManager.Core.Params;
using NetAuthManager.Core.Results;
using NetAuthManager.Core.Services;
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
public class DictionaryTypeService : BaseService<SysDictionaryType>, IDictionaryTypeService, ITransient
{
    #region 构造与注入

    private readonly IUserLoginService _userLoginService;
    private readonly IMenuResourceService _menuResourceService;
    private readonly IRepository<SysDictionaryItem> _dictionaryItemRepository;

    public DictionaryTypeService(IRepository<SysDictionaryType> entityRepository, IServiceScopeFactory scopeFactory, IUserLoginService userLoginService,
        IMenuResourceService menuResourceService, IRepository<SysDictionaryItem> dictionaryItemRepository) : base(entityRepository, scopeFactory)
    {
        _userLoginService = userLoginService;
        _menuResourceService = menuResourceService;
        _dictionaryItemRepository = dictionaryItemRepository;
    }

    #endregion 构造与注入

    /// <summary>
    /// 获取分页列表，仅启用用户
    /// 公开访问，无需权限
    /// </summary>
    public override async Task<PageResult<SysDictionaryType>> GetPageList(PageSearchParam param)
    {
        //获取权限
        var perms = await _menuResourceService.GetResourceBizAllowPermNames();

        //权限构建
        var permExpression = ExpressionParserEx<SysDictionaryType>.ParserConditionsByPerms(perms, "dicType");

        //查询条件
        var whereExpression = ExpressionParser<SysDictionaryType>.ParserConditions(param.filters, "dicType");

        //分页查询
        var list = await _entityRepository.Where(permExpression)
            .Where(whereExpression).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<SysDictionaryType>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 添加
    /// </summary>
    public async Task Add(DictionaryTypeAddParam param)
    {
        if (string.IsNullOrEmpty(param.TypeCode))
            throw new Exception("字典类型编码不能为空！");

        if (string.IsNullOrEmpty(param.TypeName))
            throw new Exception("字典类型名称不能为空！");

        if (await AnyAsync(x => x.TypeCode == param.TypeCode))
            throw new Exception($"字典类型 “{param.TypeCode}” 已存在，不能重复新增！");

        if (await AnyAsync(x => x.TypeName == param.TypeName))
            throw new Exception($"字典类型 “{param.TypeName}” 已存在，不能重复新增！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //获取最大排序
        var orderIndex = await MaxAsync(x => true, x => x.OrderIndex);

        //新增
        var model = new SysDictionaryType()
        {
            TypeCode = param.TypeCode,
            TypeName = param.TypeName,
            ExtName01 = param.ExtName01,
            ExtName02 = param.ExtName02,
            ExtName03 = param.ExtName03,
            ExtName04 = param.ExtName04,
            ExtName05 = param.ExtName05,
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
    public async Task Modify(DictionaryTypeModifyParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典类型ID不能为空！");

        if (string.IsNullOrEmpty(param.TypeCode))
            throw new Exception("字典类型编码不能为空！");

        if (string.IsNullOrEmpty(param.TypeName))
            throw new Exception("字典类型名称不能为空！");

        //历史数据
        var model = await GetByIdAsync(param.Id);
        if (model == null)
            throw new Exception($"当前修改的字典类型不存在！");

        if (await AnyAsync(x => x.TypeCode == param.TypeCode && x.Id != param.Id))
            throw new Exception($"字典类型 “{param.TypeCode}” 已存在，不能重复新增！");

        if (await AnyAsync(x => x.TypeName == param.TypeName && x.Id != param.Id))
            throw new Exception($"字典类型 “{param.TypeName}” 已存在，不能重复新增！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //事务执行
        model.TypeCode = param.TypeCode;
        model.TypeName = param.TypeName;
        model.ExtName01 = param.ExtName01;
        model.ExtName02 = param.ExtName02;
        model.ExtName03 = param.ExtName03;
        model.ExtName04 = param.ExtName04;
        model.ExtName05 = param.ExtName05;
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
    public async Task Delete(DictionaryTypeBaseParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典类型ID不能为空！");

        //历史数据
        var model = await GetByIdAsync(param.Id);
        if (model != null)
        {
            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除字典
                await _entityRepository.DeleteAsync(model);

                //删除字典项
                await _dictionaryItemRepository.Where(entity => entity.TypeId == model.Id).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 删除多个字典项
    /// </summary>
    public async Task Deletes(DictionaryTypeDeletesParam param)
    {
        if (param.Ids == null || param.Ids.Count == 0)
            throw new Exception("字典类型ID不能为空！");

        //历史数据
        var models = (from entity in _entityRepository.Entities
                     where param.Ids.Contains(entity.Id)
                     select entity).ToList();
        if (models != null && models.Count > 0)
        {
            //事务执行
            await TryTransDoTaskAsync(async (dbContext) =>
            {
                //删除字典
                await _entityRepository.DeleteAsync(models);

                //删除字典项
                await _dictionaryItemRepository.Where(entity => param.Ids.Contains(entity.TypeId)).ExecuteDeleteAsync();
            });
        }
    }

    /// <summary>
    /// 设置用户是否启用
    /// </summary>
    public async Task SetIsOpen(SetDictionaryTypeIsOpenParam param)
    {
        if (string.IsNullOrEmpty(param.Id))
            throw new Exception("字典类型ID不能为空！");

        //数据
        var model = await GetByIdAsync(param.Id);
        if (model == null)
            throw new Exception($"当前修改的字典类型不存在！");

        //登陆用户
        var loginUser = _userLoginService.GetLoginUserInfo();

        //执行更新
        model.IsOpen = param.IsOpen;
        model.UpdatorAccount = loginUser.UserAccount;
        model.UpdatorName = loginUser.UserName;
        model.UpdatedTime = DateTime.Now;
        await _entityRepository.UpdateNowAsync(model);
    }
}
