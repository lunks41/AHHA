

using AHHA.Application.CommonServices;
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

        public async Task<IEnumerable<T>> GetQueryAsync<T>(string spName, object? Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                var entities = await connection.QueryAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQuerySingleOrDefaultAsync<T>(string SpName, object? Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                var entities = await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters);
                return entities;
            }
        }

        public async Task<T> GetQueryFirstAsync<T>(string spName, object? Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                var entities = await connection.QueryFirstAsync<T>(spName, Parameters);
                return entities;
            }
        }

        public async Task<bool> GetExecuteScalarAsync(string SpName, object? Parameters, string ConStr = "DbConnection")
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                rowsAffected = await connection.ExecuteScalarAsync<int>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<DataSet> GetExecuteDataSetQuery(string storedProcedureName)
        {
            using IDbConnection connection = CreateConnection("DbConnection");
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> GetExecuteDataSetStoredProcedure(string storedProcedureName, DynamicParameters? parameters = null)
        {
            using IDbConnection connection = CreateConnection("DbConnection");
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
        private IDbConnection CreateConnection(string ConStr)
        {
            IDbConnection db = new SqlConnection(_configuration.GetConnectionString(ConStr));
            if (db.State == ConnectionState.Closed)
                db.Open();
            return db;
        }

        #endregion

    }
}
