﻿using Dapper;
using System.Data;

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
        Task<IEnumerable<T>> GetQueryAsync<T>(string RegId, string spName, object? Parameters = null);

        /// <summary>
        /// Get first row of Data By passing T as view model class by using query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SpName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<T> GetQuerySingleOrDefaultAsync<T>(string RegId, string SpName, object? Parameters = null);

        /// <summary>
        /// Get first row of Data By passing T as view model class by using query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="spName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<T> GetQueryFirstAsync<T>(string RegId, string spName, object? Parameters = null);

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="SpName"></param>
        /// <param name="Parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        Task<bool> GetExecuteScalarAsync(string RegId, string SpName, object? Parameters);

        Task<int> GetRowExecuteAsync(string RegId, string SpName);

        Task<bool> GetQuerySingleOrDefaultAsync(string RegId, string SpName);

        Task<T> GetQuerySingleOrDefaultAsyncV1(string RegId, string SpName);

        Task<bool> UpsertExecuteScalarAsync(string RegId, string QueryString);

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        Task<DataSet> GetExecuteDataSetQuery(string RegId, string storedProcedureName);

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DataSet> GetExecuteDataSetStoredProcedure(string RegId, string storedProcedureName, DynamicParameters parameters);

        /// <summary>
        /// Get table type of data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T> ExecuteStoredProcedureAsync<T>(string RegId, string storedProcedureName, DynamicParameters parameters, string outputParameterName);
    }
}