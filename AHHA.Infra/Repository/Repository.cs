

using AHHA.Application.CommonServices;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace AHHA.Infra.Repository
{
    //Unit of Work Pattern
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<bool> IsExists<Tvalue>(string key, Tvalue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _context.Set<T>().AnyAsync(lambda);
        }

        //Before update existence check
        public async Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, "Id");
            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return await _context.Set<T>().AnyAsync(lambda);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(new object[] { id });

        public async Task<T> GetById<Tid>(Tid id)
        {
            var data = await _context.Set<T>().FindAsync(id);
            if (data == null)
                return null;
            return data;
        }

        public IQueryable<T> GetAllQuery() => _dbSet.AsNoTracking();

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
