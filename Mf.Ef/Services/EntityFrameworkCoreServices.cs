using Microsoft.EntityFrameworkCore;
using Mf.Core.Interfaces;
using Mf.Core.SubEntities;
using Mf.Core.SubEntities.QueryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Mf.EfCore.Services
{
    public class EntityFrameworkCoreServices<TEntity> : IGenericService<TEntity>, IDisposable
        where TEntity : class, IEntity
    {
        protected readonly DbContext DbContext;
        public DbSet<TEntity> Entities { get; }
        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();


        public EntityFrameworkCoreServices(DbContext dbContext)
        {
            DbContext = dbContext;
            Entities = DbContext.Set<TEntity>(); // City => Cities
        }

        #region Async Method
        public virtual async Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return await Entities.FindAsync(ids, cancellationToken).AsTask();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var entityFull = await DbContext.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (saveNow)
            {
                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    entityFull.State = EntityState.Detached;
                }
            }
            return entityFull.Entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entities;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var EntityState = Entities.Update(entity);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
                EntityState.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Entities.UpdateRange(entities);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);

            return entities;
        }

        public virtual async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            try
            {
                Entities.Remove(entity);
            }
            catch (Exception ex)
            {

                throw;
            }
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Entities.RemoveRange(entities);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
            return entities;
        }
        #endregion

        #region Sync Methods
        public virtual TEntity GetById(params object[] ids)
        {
            return Entities.Find(ids);
        }

        public virtual void Add(TEntity entity, bool saveNow = true)
        {

            Entities.Add(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {

            Entities.AddRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {

            Entities.Update(entity);
            DbContext.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {

            Entities.UpdateRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {

            Entities.Remove(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {

            Entities.RemoveRange(entities);
            if (saveNow)
                DbContext.SaveChanges();
        }
        #endregion

        #region Attach & Detach
        public virtual void Detach(TEntity entity)
        {

            var entry = DbContext.Entry(entity);
            if (entry != null)
                entry.State = EntityState.Detached;
        }

        public virtual void Attach(TEntity entity)
        {

            if (DbContext.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
        }
        #endregion

        #region Explicit Loading
        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);
            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                collection.Load();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();
        }

        public void SaveChange()
        {
            DbContext.SaveChanges();
        }

        public async Task<bool> SaveChangeAsync()
        {
            await DbContext.SaveChangesAsync();
            return true;
        }


        #endregion
        public void Dispose()
        {
            DbContext.Dispose();
        }


        //TODO: Complte there
        public IEnumerable<TEntity> GetAll()
        {
            return TableNoTracking;
        }

        async Task<IEntity> IGenericService<TEntity>.GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            var res = await GetByIdAsync(cancellationToken, ids);
            return (IEntity)res;
        }

        public Task<IEnumerable<IEntity>> AddRangeAsync(IEnumerable<IEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            throw new NotImplementedException();
        }

        public async Task<IEntity> UpdateAsync(object entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var en = (TEntity)entity;
            var result = await UpdateAsync(en, cancellationToken, saveNow);
            return result;
        }

        public async Task<IEntity> AddAsync(object entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var b = (TEntity)entity;
            var res = await AddAsync(b, cancellationToken, saveNow);
            return res;
        }

        public async Task<IEntity> DeleteAsync(object entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            var en = (TEntity)entity;
            var result = await DeleteAsync(en, cancellationToken, saveNow);
            return result;
        }

        public IEnumerable<TEntity> GetList(QueryRequest? query = null)
        {
            var products = TableNoTracking.AsQueryable();
            if (query == null)
            {
                return products;
            }
            foreach (var include in query.Includes)
            {
                products = products.Include(include);
            }
            // 2. Apply Filters
            foreach (var filter in query.Filters)
            {
                products = ApplyFilter(products, filter);
            }
            // 3. Apply Skip and Take
            if (query.Skip.HasValue)
                products = products.Skip(query.Skip.Value);
            if (query.Take.HasValue)
                products = products.Take(query.Take.Value);
            return products;
        }

        private IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, Filter filter)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, filter.Field);
            var constant = Expression.Constant(Convert.ChangeType(filter.Value, property.Type));

            Expression body = filter.Operator switch
            {
                "=" => Expression.Equal(property, constant),
                ">" => Expression.GreaterThan(property, constant),
                ">=" => Expression.GreaterThanOrEqual(property, constant),
                "<" => Expression.LessThan(property, constant),
                "<=" => Expression.LessThanOrEqual(property, constant),
                "!=" => Expression.NotEqual(property, constant),
                "Contains" => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant),
                "StartsWith" => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant),
                "EndsWith" => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant),
                _ => throw new NotSupportedException($"Operator {filter.Operator} not supported.")
            };

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return query.Where(lambda);
        }


    }
}
