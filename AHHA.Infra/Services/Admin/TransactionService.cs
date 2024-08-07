using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Services.Admin
{
    public sealed class TransactionService : ITransactionService
    {
        private readonly IRepository<AdmTransaction> _repository;
        private ApplicationDbContext _context;

        public TransactionService(IRepository<AdmTransaction> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AdmTransaction>> GetUsersTransactionsAsync(Int16 CompanyId, Int16 ModuleId, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@CompanyId", CompanyId));
                parameter.Add(new SqlParameter("@ModuleId", ModuleId));
                parameter.Add(new SqlParameter("@UserId", UserId));

                var productDetails = await Task.Run(() => _context.AdmTransaction
                                .FromSqlRaw(@"exec Adm_GetUserTransactions @CompanyId,@ModuleId,@UserId", parameter.ToArray()).ToListAsync());

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Admin,
                    TransactionId = (short)Admins.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersTransactionsAsync",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AdmTransaction>> GetUsersTransactionsAllAsync(Int16 CompanyId, Int16 ModuleId, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@CompanyId", CompanyId));
                parameter.Add(new SqlParameter("@ModuleId", ModuleId));
                parameter.Add(new SqlParameter("@UserId", UserId));

                var productDetails = await Task.Run(() => _context.AdmTransaction
                                .FromSqlRaw(@"exec Adm_GetUserTransactions_All @CompanyId,@ModuleId,@UserId", parameter.ToArray()).ToListAsync());

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Admin,
                    TransactionId = (short)Admins.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersTransactionsAllAsync",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

    }
}
