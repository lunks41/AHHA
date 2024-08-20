﻿using AHHA.Application.CommonServices;
using AHHA.Core.Common;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AHHA.Infra.Repository
{
    //Unit of Work Pattern
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IConfiguration _configuration;

        public Repository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _configuration = configuration;
        }

        public async Task<IEnumerable<T>> GetQueryAsync<T>(string RegId, string spName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection(RegId))
            {
                var entities = await connection.QueryAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQuerySingleOrDefaultAsync<T>(string RegId, string SpName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection(RegId))
            {
                var entities = await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQueryFirstAsync<T>(string RegId, string spName, object? Parameters)
        {
            using (IDbConnection connection = CreateConnection(RegId))
            {
                var entities = await connection.QueryFirstAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<bool> GetExecuteScalarAsync(string RegId, string SpName, object? Parameters)
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection(RegId))
            {
                rowsAffected = await connection.ExecuteScalarAsync<int>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<DataSet> GetExecuteDataSetQuery(string RegId, string storedProcedureName)
        {
            using IDbConnection connection = CreateConnection(RegId);
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetExecuteDataSetStoredProcedure(string RegId, string storedProcedureName, DynamicParameters? parameters = null)
        {
            using IDbConnection connection = CreateConnection(RegId);
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (var paramName in parameters.ParameterNames)
                    {
                        var paramValue = parameters.Get<object>(paramName);
                        sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@" + paramName, paramValue);
                    }
                }
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        #region Private Methods

        private IDbConnection CreateConnection(string RegId)
        {
            DBGetConnection dBGetConnection = new DBGetConnection();
            try
            {
                var ConnectionStringName = dBGetConnection.GetconnectionDB(RegId);

                IDbConnection db = new SqlConnection(_configuration.GetConnectionString(ConnectionStringName));
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dBGetConnection = null;
            }
        }

        #endregion Private Methods
    }
}