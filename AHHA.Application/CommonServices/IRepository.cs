
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
        /// <summary>
        /// Get List of Data By passing T as view model class by using query
        /// </summary>
        /// <typeparam name="T"> return type as a object which was you provide</typeparam>
        /// <param name="spName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns name="T"> return type as int,string or object etc which you provide on method</returns>
        Task<IEnumerable<T>> GetQueryAsync<T>(string spName, object? Parameters = null, string ConStr = "DbConnection");

        /// <summary>
        /// Get first row of Data By passing T as view model class by using query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SpName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<T> GetQuerySingleOrDefaultAsync<T>(string SpName, object? Parameters = null, string ConStr = "DbConnection");

        /// <summary>
        /// Get first row of Data By passing T as view model class by using query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="spName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<T> GetQueryFirstAsync<T>(string spName, object? Parameters=null, string ConStr = "DbConnection");

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="SpName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<bool> GetExecuteScalarAsync(string SpName, object? Parameters, string ConStr = "DbConnection");

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        Task<DataSet> GetExecuteDataSetQuery(string storedProcedureName);

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DataSet> GetExecuteDataSetStoredProcedure(string storedProcedureName, DynamicParameters parameters = null);
    }
}
