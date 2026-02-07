using FamilyRestraunt.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyRestraunt.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected  ApplicationDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
       public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options != null)
            {
                if (options.HasWhere())
                {
                    query = query.Where(options.where);
                }
                if (options.HasOrderby())
                {
                    foreach (var orderby in options.Orderby!)
                    {
                        query = query.OrderBy(orderby);
                    }
                }
                var includes = options.GetIncludes();
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                var entityType = _context.Model.FindEntityType(typeof(T));
                if (entityType == null)
                {
                    return null;
                }
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey == null || primaryKey.Properties.Count == 0)
                {
                    return null;
                }
                // CA1826: Use index instead of FirstOrDefault
                var key = primaryKey.Properties[0];
                string primaryKeyName = key.Name;
                return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
            }
            return null;
        }
        public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options != null)
            {
                if (options.HasWhere())
                {
                    query = query.Where(options.where);
                }
                if (options.HasOrderby())
                {
                    foreach (var orderby in options.Orderby!)
                    {
                        query = query.OrderBy(orderby);
                    }
                }
                var includes = options.GetIncludes();
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                query = query.Where(e => EF.Property<TKey>(e, propertyName)!.Equals(id));
            }
            return await query.ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);    
            await _context.SaveChangesAsync();
        }

    }
}
