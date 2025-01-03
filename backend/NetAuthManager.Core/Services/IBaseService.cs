namespace NetAuthManager.Core.Services;

/// <summary>
/// 联行信息服务
/// </summary>
public interface IBaseService<TEntity, TKey>
{
    #region 查询

    /// <summary>
    /// 获取分页列表
    /// </summary>
    Task<PageResult<TEntity>> GetAllPageList(PageSearchParam param);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    Task<PageResult<TEntity>> GetPageList(PageSearchParam param);

    /// <summary>
    /// 获取所有数据
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> GetAllList(BaseSearchParam param);

    /// <summary>
    /// 获取所有数据列表
    /// </summary>
    Task<int> GetTotalCount(BaseSearchParam param);

    /// <summary>
    /// 获取分页列表
    /// </summary>
    Task<TEntity> GetByIdAsync(TKey id);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    TEntity GetFirst(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    bool Any(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    List<TEntity> GetList();

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    Task<List<TEntity>> GetListAsync();

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    List<TEntity> GetList(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 获取指定主键对应的数据
    /// </summary>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression);

    #endregion 查询

    #region 新增

    /// <summary>
    /// 保存实体
    /// </summary>
    Task InsertAsync(TEntity entity);

    /// <summary>
    /// 保存实体
    /// </summary>
    void Insert(TEntity entity);

    /// <summary>
    /// 保存实体
    /// </summary>
    void InsertOrUpdate(TEntity entity);

    /// <summary>
    /// 保存实体
    /// </summary>
    Task InsertOrUpdateAsync(TEntity entity);

    /// <summary>
    /// 保存实体
    /// </summary>
    void InsertOrUpdate(TEntity entity, Expression<Func<TEntity, bool>> keyExpression);

    /// <summary>
    /// 保存实体
    /// </summary>
    Task InsertOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> keyExpression);

    #endregion 新增

    #region 更新

    /// <summary>
    /// 保存实体
    /// </summary>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// 保存实体
    /// </summary>
    void Update(TEntity entity);

    #endregion 更新

    #region 删除

    /// <summary>
    /// 删除实体
    /// </summary>
    Task DeleteByIdAsync(TKey id);

    /// <summary>
    /// 删除多个实体
    /// </summary>
    Task DeleteByIdsAsync(TKey[] ids);

    #endregion 删除
}
