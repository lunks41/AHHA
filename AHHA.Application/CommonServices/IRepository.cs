
using AHHA.Core.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.CommonServices
{
    //Unit of Work Pattern
    public interface IRepository<T> where T : class
    {
        #region Enitity Framework
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetById<Tid>(Tid id);
        IQueryable<T> GetAllQuery();
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value);

        #endregion

        #region SQL

        Task<T> QuerySingleORDefaultAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection");
        Task<IEnumerable<T>> QueryIEnumerableAsync<T, P>(string spName, P Parameters);
        Task<T> QueryFirstAsync<T, P>(string spName, P Parameters);

        Task<(IEnumerable<T> entities, int totalCount)> QueryMultiplePagingEntityAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<(IEnumerable<T> entities, int totalCount)> QueryMultipleDtoPagingAsync<T>(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<IEnumerable<T>> QueryAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<IEnumerable<T>> QueryDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection");
        Task<T> QueryDetailDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection");

        Task<T> QuerySingleOrDefaultAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<SqlResponce> QuerySqlResponseModelAsync(string SpName, Object Parameters, string ConStr = "DbConnection");
        Task<bool> ExecuteScalarAsync(string SpName, Object Parameters, string ConStr = "DbConnection");

        Task<DataSet> ExecuteDataSetStoredProcedure(string storedProcedureName, DynamicParameters parameters = null);
        Task<DataSet> ExecuteDataSetQuery(string storedProcedureName);

        Task<IEnumerable<SqlResponceIds>> GetAllFromSqlExecuteReaderAsyn<T, P>(string spName, P Parameters);


        Task<IEnumerable<T>> GetAllAsync<T, P>(string spName, P Parameters);
        Task<IEnumerable<T>> GetByIdAsync<T, P>(string spName, P Parameters);
        Task<SqlResponce> UpsertAsync<T>(string spName, T Parameters);

        #endregion


    }
}
