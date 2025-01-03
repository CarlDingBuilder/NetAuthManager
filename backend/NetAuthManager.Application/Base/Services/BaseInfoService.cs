using System.Linq.Expressions;
using Furion.LinqBuilder;
using NetAuthManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Furion.DatabaseAccessor;
using NetAuthManager.Core.Services;

namespace NetAuthManager.Application.Services;

/// <summary>
/// 基础服务
/// </summary>
public class BaseInfoService<TEntity> : BaseService<TEntity>, ITransient where TEntity : BaseGuidInfoEntity, new()
{
    #region 构造和注入

    protected readonly IUserLoginService _userLoginService;
    public BaseInfoService(IRepository<TEntity> entityRepository, IServiceScopeFactory scopeFactory, IUserLoginService userLoginService) : base(entityRepository, scopeFactory)
    {
        _userLoginService = userLoginService;
    }

    #endregion 构造和注入

    #region 新增

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    public override void InsertOrUpdate(TEntity entity)
    {
        if (entity == null) return;
        var userInfo = _userLoginService.GetLoginUserInfo();

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            entity.CreatorAccount = userInfo.UserAccount;
            entity.CreatorName = userInfo.UserName;
            entity.CreatedTime = DateTime.Now;
            _entityRepository.InsertNow(entity);
        }
        else //更新的情况
        {
            entity.UpdatorAccount = userInfo.UserAccount;
            entity.UpdatorName = userInfo.UserName;
            entity.UpdatedTime = DateTime.Now;
            _entityRepository.UpdateNow(entity);
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    public override async Task InsertOrUpdateAsync(TEntity entity)
    {
        if (entity == null) return;
        var userInfo = _userLoginService.GetLoginUserInfo();

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            entity.CreatorAccount = userInfo.UserAccount;
            entity.CreatorName = userInfo.UserName;
            entity.CreatedTime = DateTime.Now;
            await _entityRepository.InsertNowAsync(entity);
        }
        else //更新的情况
        {
            entity.UpdatorAccount = userInfo.UserAccount;
            entity.UpdatorName = userInfo.UserName;
            entity.UpdatedTime = DateTime.Now;
            await _entityRepository.UpdateNowAsync(entity);
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    /// <param name="keyExpression">主键表达式：x => x.Code == entity.Code</param>
    public override void InsertOrUpdate(TEntity entity, Expression<Func<TEntity, bool>> keyExpression)
    {
        if (entity == null) return;
        var userInfo = _userLoginService.GetLoginUserInfo();

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //判定
            if (_entityRepository.Any(keyExpression))
                throw new CustomException("已存在相同主键的数据");

            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            entity.CreatorAccount = userInfo.UserAccount;
            entity.CreatorName = userInfo.UserName;
            entity.CreatedTime = DateTime.Now;
            _entityRepository.InsertNow(entity);
        }
        else //更新的情况
        {
            //判定
            if (_entityRepository.Any(keyExpression.And(x => x.Id != entity.Id)))
                throw new CustomException("已存在相同主键的数据");

            var oldEntity = _entityRepository.First(x => x.Id == entity.Id);
            var primarykeys = _entityRepository.EntityType.GetKeys();

            var isSame = true;
            foreach (var primarykey in primarykeys)
            {
                var value = entity.GetType().GetProperty(primarykey.GetName()).GetValue(entity, null);
                var oldValue = oldEntity.GetType().GetProperty(primarykey.GetName()).GetValue(oldEntity, null);
                if (!value.Equals(oldValue)) isSame = false;
            }

            //主键不同则删除并插入，所以调用该方法前要先校验，否则会被覆盖
            if (!isSame)
            {
                //先删除再新增数据
                TryTransDo((dbContext) =>
                {
                    _entityRepository.Delete(entity.Id);

                    entity.Id = BaseGuidEntity.GetNewGuid();
                    entity.CreatorAccount = userInfo.UserAccount;
                    entity.CreatorName = userInfo.UserName;
                    entity.CreatedTime = DateTime.Now;
                    _entityRepository.Insert(entity);
                });
            }
            else
            {
                entity.UpdatorAccount = userInfo.UserAccount;
                entity.UpdatorName = userInfo.UserName;
                entity.UpdatedTime = DateTime.Now;
                _entityRepository.UpdateNow(entity);
            }
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    /// <param name="keyExpression">主键表达式：x => x.Code == entity.Code</param>
    public override async Task InsertOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression)
    {
        if (entity == null) return;
        var userInfo = _userLoginService.GetLoginUserInfo();

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //判定
            if (await _entityRepository.AnyAsync(keyExpression))
                throw new CustomException("已存在相同主键的数据");

            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            entity.CreatorAccount = userInfo.UserAccount;
            entity.CreatorName = userInfo.UserName;
            entity.CreatedTime = DateTime.Now;
            await _entityRepository.InsertNowAsync(entity);
        }
        else //更新的情况
        {
            //判定
            if (await _entityRepository.AnyAsync(keyExpression.And(x => x.Id != entity.Id)))
                throw new CustomException("已存在相同主键的数据");

            var oldEntity = await _entityRepository.FirstAsync(x => x.Id == entity.Id);
            var primarykeys = _entityRepository.EntityType.GetKeys();

            var isSame = true;
            foreach(var primarykey in primarykeys)
            {
                var value = entity.GetType().GetProperty(primarykey.GetName()).GetValue(entity, null);
                var oldValue = oldEntity.GetType().GetProperty(primarykey.GetName()).GetValue(oldEntity, null);
                if (!value.Equals(oldValue)) isSame = false;
            }

            //主键不同则删除并插入，所以调用该方法前要先校验，否则会被覆盖
            if (!isSame)
            {
                //先删除再新增数据
                TryTransDo(async (dbContext) =>
                {
                    await _entityRepository.DeleteAsync(entity.Id);

                    entity.Id = BaseGuidEntity.GetNewGuid();
                    entity.CreatorAccount = userInfo.UserAccount;
                    entity.CreatorName = userInfo.UserName;
                    entity.CreatedTime = DateTime.Now;
                    await _entityRepository.InsertAsync(entity);
                });
            }
            else
            {
                entity.UpdatorAccount = userInfo.UserAccount;
                entity.UpdatorName = userInfo.UserName;
                entity.UpdatedTime = DateTime.Now;
                await _entityRepository.UpdateNowAsync(entity);
            }
        }
    }

    #endregion 新增
}
