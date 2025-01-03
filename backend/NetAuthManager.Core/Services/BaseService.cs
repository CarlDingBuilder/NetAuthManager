using System.Linq.Expressions;
using Furion.LinqBuilder;
using NetAuthManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using NetAuthManager.Core.Results;
using System.Threading.Tasks;
using NetAuthManager.Core.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Furion.DatabaseAccessor;
using NetAuthManager.Core.Common.Enums;
using System.Transactions;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NetAuthManager.Core.Services;

/// <summary>
/// 基础服务
/// </summary>
public class BaseService<TEntity> : IBaseService<TEntity, string>, ITransient where TEntity : BaseGuidEntity, new()
{
    #region 构造和注入

    protected readonly IRepository<TEntity> _entityRepository;
    protected readonly IServiceScopeFactory _scopeFactory;

    public BaseService(IRepository<TEntity> entityRepository, IServiceScopeFactory scopeFactory)
    {
        _entityRepository = entityRepository;
        _scopeFactory = scopeFactory;
    }

    #endregion 构造和注入

    #region 查询

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public virtual async Task<PageResult<TEntity>> GetAllPageList(PageSearchParam param)
    {
        var @where = ExpressionParser<TEntity>.ParserConditions(param.filters);
        var list = await _entityRepository.Where(where).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<TEntity>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public virtual async Task<PageResult<TEntity>> GetPageList(PageSearchParam param)
    {
        var @where = ExpressionParser<TEntity>.ParserConditions(param.filters);
        var list = await _entityRepository.Where(where).OrderBy(param.sorts).ToPagedListAsync(param.pageNo, param.pageSize);
        return PageResult<TEntity>.Get(list: list.Items.ToList(), total: list.TotalCount);
    }

    /// <summary>
    /// 获取所有数据列表
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllList(BaseSearchParam param)
    {
        var @where = ExpressionParser<TEntity>.ParserConditions(param.filters);
        return await _entityRepository.DetachedEntities.Where(@where).OrderBy(param.sorts).ToListAsync();//where, param.sorts
    }

    /// <summary>
    /// 获取所有数据列表
    /// </summary>
    public virtual async Task<int> GetTotalCount(BaseSearchParam param)
    {
        var @where = ExpressionParser<TEntity>.ParserConditions(param.filters);
        return await _entityRepository.CountAsync(where);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual async Task<TEntity> GetByIdAsync(string id)
    {
        return await _entityRepository.DetachedEntities.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _entityRepository.DetachedEntities.FirstOrDefault(whereExpression);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _entityRepository.DetachedEntities.FirstOrDefaultAsync(whereExpression);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual bool Any(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _entityRepository.Entities.Any(whereExpression);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _entityRepository.DetachedEntities.AnyAsync(whereExpression);
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual List<TEntity> GetList()
    {
        return _entityRepository.DetachedEntities.ToList();
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual async Task<List<TEntity>> GetListAsync()
    {
        return await _entityRepository.DetachedEntities.ToListAsync();
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _entityRepository.DetachedEntities.Where(whereExpression).ToList();
    }

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _entityRepository.DetachedEntities.Where(whereExpression).ToListAsync();
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public virtual TResult Max<TResult>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TResult>> expression)
    {
        return _entityRepository.Entities.Where(whereExpression).Max(expression);
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TResult>> expression)
    {
        var hasData = await _entityRepository.Entities.Where(whereExpression).AnyAsync();
        if (hasData)
        {
            return await _entityRepository.Entities.Where(whereExpression).MaxAsync(expression);
        }
        else
        {
            return default(TResult);
        }
    }

    #endregion 查询

    #region 新增

    /// <summary>
    /// 保存实体
    /// </summary>
    public virtual async Task InsertAsync(TEntity entity)
    {
        await _entityRepository.InsertNowAsync(entity);
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    public virtual void Insert(TEntity entity)
    {
        _entityRepository.InsertNow(entity);
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    public virtual void InsertOrUpdate(TEntity entity)
    {
        if (entity == null) return;

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            _entityRepository.InsertNow(entity);
        }
        else //更新的情况
        {
            _entityRepository.UpdateNow(entity);
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    public virtual async Task InsertOrUpdateAsync(TEntity entity)
    {
        if (entity == null) return;

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
            await _entityRepository.InsertNowAsync(entity);
        }
        else //更新的情况
        {
            await _entityRepository.UpdateNowAsync(entity);
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    /// <param name="keyExpression">主键表达式：x => x.Code == entity.Code</param>
    public virtual void InsertOrUpdate(TEntity entity, Expression<Func<TEntity, bool>> keyExpression)
    {
        if (entity == null) return;

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //判定
            if (_entityRepository.Any(keyExpression))
                throw new CustomException("已存在相同主键的数据");

            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
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
                TryTransDo(((dbContext) =>
                {
                    _entityRepository.Delete(entity.Id);
                    _entityRepository.Insert(entity);
                }));
            }
            else
            {
                _entityRepository.UpdateNow(entity);
            }
        }
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">当前实体</param>
    /// <param name="keyExpression">主键表达式：x => x.Code == entity.Code</param>
    public virtual async Task InsertOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression)
    {
        if (entity == null) return;

        //新增的情况
        if (string.IsNullOrEmpty(entity.Id))
        {
            //判定
            if (await _entityRepository.AnyAsync(keyExpression))
                throw new CustomException("已存在相同主键的数据");

            //添加
            entity.Id = BaseGuidEntity.GetNewGuid();
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
                await TryTransDoTaskAsync(async (dbContext) =>
                {
                    await _entityRepository.DeleteAsync(entity.Id);
                    await _entityRepository.InsertAsync(entity);
                });
            }
            else
            {
                await _entityRepository.UpdateNowAsync(entity);
            }
        }
    }

    #endregion 新增

    #region 修改

    /// <summary>
    /// 保存实体
    /// </summary>
    public virtual async Task UpdateAsync(TEntity entity)
    {
        await _entityRepository.UpdateNowAsync(entity);
    }

    /// <summary>
    /// 保存实体
    /// </summary>
    public virtual void Update(TEntity entity)
    {
        _entityRepository.UpdateNow(entity);
    }

    #endregion

    #region 删除

    /// <summary>
    /// 删除实体
    /// </summary>
    public virtual async Task DeleteByIdAsync(string id)
    {
        await _entityRepository.DeleteNowAsync(id);
    }

    /// <summary>
    /// 删除多个实体
    /// </summary>
    public virtual async Task DeleteByIdsAsync(string[] ids)
    {
        await _entityRepository.Entities.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
    }

    #endregion 删除

    #region 事务方法

    /// <summary>
    /// 执行某操作
    /// </summary>
    public virtual void TryTransDo(Func<DbContext, bool> dnFunc)
    {
        // 获取数据库上下文
        var context = Db.GetDbContext();
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var success = dnFunc(context);
                if (success)
                {
                    // 保存所有更改
                    context.SaveChanges();

                    // 提交事务
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 执行某操作
    /// </summary>
    public virtual void TryTransDo(TryTransDoDelegate dnFunc)
    {
        // 获取数据库上下文
        var context = Db.GetDbContext();
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                //示例1
                //_entityRepository.DeleteNow(entity.Id);

                //示例2
                //_entityRepository.Delete(entity.Id);
                //_entityRepository.SaveNow();

                //示例3
                //context.SaveChanges();

                //执行某操作
                dnFunc(context);

                // 保存所有更改
                context.SaveChanges();

                // 提交事务
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 执行某操作
    /// </summary>
    public virtual T TryTransDoReturn<T>(TryTransDoReturnDelegate<T> dnFunc)
    {
        // 获取数据库上下文
        var context = Db.GetDbContext();
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                //示例1
                //_entityRepository.DeleteNow(entity.Id);

                //示例2
                //_entityRepository.Delete(entity.Id);
                //_entityRepository.SaveNow();

                //示例3
                //context.SaveChanges();

                //执行某操作
                var result = dnFunc(context);

                // 保存所有更改
                context.SaveChanges();

                // 提交事务
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 执行某操作
    /// </summary>
    public virtual async Task TryTransDoTaskAsync(TryTransDoTaskAsyncDelegate dnFunc)
    {
        // 获取数据库上下文
        var context = Db.GetDbContext();
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                //示例1
                //_entityRepository.DeleteNow(entity.Id);

                //示例2
                //_entityRepository.Delete(entity.Id);
                //_entityRepository.SaveNow();

                //示例3
                //context.SaveChanges();

                //执行某操作
                await dnFunc(context);

                // 保存所有更改
                context.SaveChanges();

                // 提交事务
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 执行某操作
    /// </summary>
    public virtual async Task<T> TryTransDoTaskReturnAsync<T>(TryTransDoTaskReturnDelegate<T> dnFunc)
    {
        // 获取数据库上下文
        var context = Db.GetDbContext();
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                //示例1
                //_entityRepository.DeleteNow(entity.Id);

                //示例2
                //_entityRepository.Delete(entity.Id);
                //_entityRepository.SaveNow();

                //示例3
                //context.SaveChanges();

                //执行某操作
                var result = await dnFunc(context);

                // 保存所有更改
                context.SaveChanges();

                // 提交事务
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 执行某操作的委托类型
    /// </summary>
    public delegate void TryTransDoDelegate(DbContext dbContext);
    /// <summary>
    /// 执行某操作的委托类型
    /// </summary>
    public delegate T TryTransDoReturnDelegate<T>(DbContext dbContext);
    /// <summary>
    /// 执行某操作的委托类型
    /// </summary>
    public delegate Task TryTransDoTaskAsyncDelegate(DbContext dbContext);
    /// <summary>
    /// 执行某操作的委托类型
    /// </summary>
    public delegate Task<T> TryTransDoTaskReturnDelegate<T>(DbContext dbContext);

    #endregion 事务方法

    #region 异步数据库任务

    /// <summary>
    /// 尝试业务执行
    /// </summary>
    public virtual async Task TryAsyncDoDelegate<TType>(TryTransDoTaskDelegate<TType> dnFunc) where TType : class, IDisposable
    {
        await Task.Run(async () =>
        {
            using (var scope = _scopeFactory.CreateAsyncScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<TType>())
                {
                    await dnFunc(dbContext, scope.ServiceProvider);
                }
            }
        });
    }

    /// <summary>
    /// 尝试业务执行并返回
    /// </summary>
    public virtual async Task<T> TryAsyncDoReturnDelegate<TType, T>(TryTransDoTaskReturnDelegate<TType, T> dnFunc) where TType : class, IDisposable
    {
        return await Task.Run(async () =>
        {
            using (var scope = _scopeFactory.CreateAsyncScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<TType>())
                {
                    return await dnFunc(dbContext, scope.ServiceProvider);
                }
            }
        });
    }
    public delegate Task TryTransDoTaskDelegate<TType>(TType dbContext, IServiceProvider serviceProvider);
    public delegate Task<T> TryTransDoTaskReturnDelegate<TType, T>(TType dbContext, IServiceProvider serviceProvider);

    #endregion
}
