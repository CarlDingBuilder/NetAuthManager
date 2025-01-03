using NetAuthManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Repositorys
{
    public interface IBaseRepository<TEntity, TDbContextLocator> : IPrivateRepository<TEntity>
    where TEntity : class, IPrivateEntity, new()
    where TDbContextLocator : class, IDbContextLocator
    {
    }

    public interface IBaseRepository<TEntity> : IBaseRepository<TEntity, MasterDbContextLocator>
        where TEntity : class, IPrivateEntity, new()
    {
    }
}
