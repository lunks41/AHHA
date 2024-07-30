


using AHHA.Application.CommonServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AHHA.Infra.Repository
{
    //Unit of Work Pattern
    internal class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        private readonly IConfiguration _configuration;

        public QueryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Methods
        public async Task<(IEnumerable<T> entities, int totalCount)> QueryMultiplePagingEntityAsync(string SpName, Object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                using var multi = await connection.QueryMultipleAsync(SpName, Parameters, commandType: CommandType.StoredProcedure);
                var entities = multi.Read<T>().ToList();
                var totalCount = multi.Read<int>().FirstOrDefault();
                return (entities, totalCount);
            }
        }
        public async Task<(IEnumerable<T> entities, int totalCount)> QueryMultipleDtoPagingAsync<T>(string SpName, Object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                using var multi = await connection.QueryMultipleAsync(SpName, Parameters, commandType: CommandType.StoredProcedure);
                var entities = multi.Read<T>().ToList();
                var totalCount = multi.Read<int>().FirstOrDefault();
                return (entities, totalCount);
            }
        }
        public async Task<IEnumerable<T>> QueryAsync(string SpName, Object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                return await connection.QueryAsync<T>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<IEnumerable<T>> QueryDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                return await connection.QueryAsync<T>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<T> QueryDetailDtoAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                return await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> QueryDetailDtoAsyncV1<T>(string SpName, object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                return await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters);
            }
        }

        public async Task<T> QuerySingleOrDefaultAsync(string SpName, Object Parameters, string ConStr = "DbConnection")
        {
            using IDbConnection connection = CreateConnection(ConStr);
            return await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<SqlResponce> QuerySqlResponseModelAsync(string SpName, Object Parameters, string ConStr = "DbConnection")
        {
            using IDbConnection connection = CreateConnection(ConStr);
            return await connection.QuerySingleOrDefaultAsync<SqlResponce>(SpName, Parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<bool> ExecuteScalarAsync(string SpName, object Parameters, string ConStr = "DbConnection")
        {
            var rowsAffected = 0;
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                rowsAffected = await connection.ExecuteScalarAsync<int>(SpName, Parameters, commandType: CommandType.StoredProcedure);
            }
            return rowsAffected > 0 ? true : false;
        }

        public async Task<IEnumerable<SqlMissingResponce>> GetAllFromSqlExecuteReaderAsyn<T, P>(string spName, P Parameters)
        {
            List<SqlMissingResponce> items = new List<SqlMissingResponce>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
                {
                    connection.Open();
                    using (var reader = connection.ExecuteReader(spName,Parameters))
                    {
                        if (reader.FieldCount > 0)
                        {
                            while (reader.Read())
                            {
                                SqlMissingResponce obj = new SqlMissingResponce
                                {
                                    IsExist = reader.GetInt32(reader.GetOrdinal("IsExist")),
                                };

                                items.Add(obj);
                            }
                        }
                    }
                }
                return items;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataSet> ExecuteDataSetQuery(string storedProcedureName)
        {
            using IDbConnection connection = CreateConnection("DbConnection");
            var dataSet = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(storedProcedureName, connection as SqlConnection))
            {
                sqlDataAdapter.Fill(dataSet);
            }
            return await Task.FromResult(dataSet);
        }

        public async Task<DataSet> ExecuteDataSetStoredProcedure(string storedProcedureName, DynamicParameters parameters = null)
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

        public async Task<IEnumerable<T>> GetAllFromSqlQueryAsync<T, P>(string spName, P Parameters)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
                {
                    connection.Open();
                    var entities = await connection.QueryAsync<T>(spName, Parameters);
                    return entities;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetAllFromSqlQueryAsyncV1<T, P>(string spName, P Parameters)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
                {
                    connection.Open();
                    var entities = await connection.QueryFirstAsync<T>(spName);
                    return entities;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T, P>(string spName, P Parameters)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
                {
                    connection.Open();
                    var entities = await connection.QueryAsync<T>(spName, Parameters, commandType: CommandType.StoredProcedure);
                    return entities;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetByIdAsync<T, P>(string spName, P Parameters)
        {

            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection"));
            return await connection.QueryAsync<T>(spName, Parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<SqlResponce> UpsertAsync<T>(string spName, T parameters)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection"));
            var result = await connection.QuerySingleOrDefaultAsync<SqlResponce>(spName, parameters, commandType: CommandType.StoredProcedure);

            var resultSQLReponce = new SqlResponce
            {
                Id = result?.Id ?? 0,
                Msg = result?.Msg ?? "Unknown"
            };

            return resultSQLReponce;
        }

        #endregion

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
