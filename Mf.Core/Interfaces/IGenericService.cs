using Mf.Core.SubEntities;
using Mf.Core.SubEntities.QueryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mf.Core.SharedKernel;

namespace Mf.Core.Interfaces
{
    public interface IGenericService<out TEntity> where TEntity : BaseEntity
    {
        public IEnumerable<TEntity> GetAll();

        public IEnumerable<TEntity> GetList(QueryRequest? query = null);

        #region Async MotimatDatabaseCore
        public Task<IEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);

        public Task<IEnumerable<IEntity>> AddRangeAsync(IEnumerable<IEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        public Task<IEntity> UpdateAsync(object entity, CancellationToken cancellationToken, bool saveNow = true);


        public Task<IEntity> DeleteAsync(object entity, CancellationToken cancellationToken, bool saveNow = true);

        public Task<IEntity> AddAsync(object entity, CancellationToken cancellationToken, bool saveNow = true);

        #endregion



    }
}
