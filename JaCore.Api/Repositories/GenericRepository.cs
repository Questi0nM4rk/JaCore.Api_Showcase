using JaCore.Api.Data;
using JaCore.Api.Entities.Interfaces;
using JaCore.Api.Extensions; // For IQueryableExtensions
using JaCore.Api.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace JaCore.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            // This method might be called by FindAsync if id is not directly supported by _dbSet.FindAsync (e.g. composite key)
            // However, for single primary key, FindAsync is generally preferred.
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T?> GetByIdAsync(object id, string? includeProperties = null)
        {
            // Primary key property name is needed to build the expression.
            // This is a common way to get it for EF Core entities.
            var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties
                            .Select(x => x.Name).SingleOrDefault();

            if (keyName == null) 
            {
                // Fallback or throw if primary key cannot be determined
                // For now, fallback to FindAsync which might not support includes well here.
                // Or, if we know 'id' is the only PK name we support, we can hardcode it (not ideal).
                return await _dbSet.FindAsync(id); // This won't use includeProperties if FindAsync is used.
            }

            IQueryable<T> query = _dbSet;
            query = query.IncludeProperties(includeProperties); // Apply includes first

            // Build predicate e.g. x => x.Id == id
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, keyName);
            var constant = Expression.Constant(id);
            var body = Expression.Equal(member, constant);
            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            
            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            IQueryable<T> query = _dbSet;
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsRemoved);
            }
            if (typeof(IDisableable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((IDisableable)e).IsDisabled);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(predicate);
            query = query.IncludeProperties(includeProperties); // Use extension method
            return await query.AsNoTracking().ToListAsync();
        }
        
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(predicate);
            query = query.IncludeProperties(includeProperties); // Use extension method
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        
        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(T entity)
        {
            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsRemoved = true;
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }
        
        public virtual async Task RemoveAsync(object id)
        {
            T? entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            return await _dbSet.CountAsync(predicate);
        }
        
        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            return _dbSet.AddRangeAsync(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable softDeletableEntity)
                {
                    softDeletableEntity.IsRemoved = true;
                    _dbSet.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _dbSet.Remove(entity);
                }
            }
        }
    }
} 