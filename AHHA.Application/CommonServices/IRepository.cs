
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.CommonServices
{
    //Unit of Work Pattern
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetById<Tid>(Tid id);
        IQueryable<T> GetAllQuery();
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value);

        
    }
}
