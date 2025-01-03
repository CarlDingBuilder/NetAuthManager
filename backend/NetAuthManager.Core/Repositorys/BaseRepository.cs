using NetAuthManager.Core.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Repositorys
{
    /// <summary>
    /// 默认数据库自定义仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : BaseRepository<TEntity, MasterDbContextLocator>, IBaseRepository<TEntity>, IScoped
        where TEntity : BaseGuidInfoEntity, IPrivateEntity, new()
    {
        public BaseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

    /// <summary>
    /// 自定义仓储实现类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContextLocator"></typeparam>
    public class BaseRepository<TEntity, TDbContextLocator> : PrivateRepository<TEntity>, IBaseRepository<TEntity, TDbContextLocator>, IScoped
        where TEntity : BaseGuidInfoEntity, IPrivateEntity, new()
        where TDbContextLocator : class, IDbContextLocator
    {
        /// <summary>
        /// 实现基类构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        public BaseRepository(IServiceProvider serviceProvider)
            : base(typeof(TDbContextLocator), serviceProvider)
        {
        }

        /// <summary>
        /// 自定义方法
        /// </summary>
        public void MyMethod()
        {
            throw new System.NotImplementedException();
        }
    }
}
