

using AHHA.Application.CommonServices;
using AHHA.Core.Common;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq.Expressions;

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

        #region Enitity Framework

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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _dbSet.AddAsync(entity);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return entity;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();
                    throw;
                }
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

        #endregion

        #region SQL

        public async Task<T> QuerySingleORDefaultAsync<T>(string SpName, object Parameters, string ConStr = "DbConnection")
        {
            using (IDbConnection connection = CreateConnection(ConStr))
            {
                var entities = await connection.QuerySingleOrDefaultAsync<T>(SpName, Parameters);
                return entities;
            }
        }

        public async Task<IEnumerable<T>> QueryIEnumerableAsync<T, P>(string spName, P Parameters)
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

        public async Task<T> QueryFirstAsync<T, P>(string spName, P Parameters)
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

        public async Task<IEnumerable<SqlResponceIds>> GetAllFromSqlExecuteReaderAsyn<T, P>(string spName, P Parameters)
        {

            List<SqlResponceIds> items = new List<SqlResponceIds>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConnection")))
                {
                    connection.Open();
                    using (var reader = connection.ExecuteReader(spName, Parameters))
                    {
                        if (reader.FieldCount > 0)
                        {
                            while (reader.Read())
                            {
                                SqlResponceIds obj = new SqlResponceIds
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

        #endregion

    }
}
