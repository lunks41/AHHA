
using AHHA.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.CommonServices
{
    //Unit of Work Pattern
    public interface IQueryRepository<T> where T : class
    {
        Task<(IEnumerable<T> entities, int totalCount)> QueryMultiplePagingEntityAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<(IEnumerable<T> entities, int totalCount)> QueryMultipleDtoPagingAsync<T>(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<IEnumerable<T>> QueryAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<IEnumerable<T>> QueryDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection");
        Task<T> QueryDetailDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection"); 
        Task<T> QueryDetailDtoAsyncV1<T>(string SpName, object Parameters, string ConStr = "DbConnection"); 
        Task<T> QuerySingleOrDefaultAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<SqlResponce> QuerySqlResponseModelAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<bool> ExecuteScalarAsync(string SpName, Object Parameters, string ConStr = "DbConnection");

        Task<IEnumerable<T>> GetAllFromSqlQueryAsync<T, P>(string spName, P Parameters);
        Task<T> GetAllFromSqlQueryAsyncV1<T, P>(string spName, P Parameters);
        Task<IEnumerable<T>> GetAllAsync<T, P>(string spName, P Parameters);
        Task<IEnumerable<T>> GetByIdAsync<T, P>(string spName, P Parameters);
        Task<SqlResponce> UpsertAsync<T>(string spName, T Parameters);
    }
}
