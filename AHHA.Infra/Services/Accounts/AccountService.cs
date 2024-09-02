using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Infra.Data;
using Dapper;
using System.Data;

namespace AHHA.Infra.Services.Accounts
{
    public sealed class AccountService : IAccountService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;

        public AccountService(IRepository<dynamic> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        //Document Id Generation
        public async Task<Int64> GenrateDocumentId(string RegId, Int16 ModuleId, Int16 TransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inModuleId", ModuleId, DbType.Int16);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            parameters.Add("@OUTPUT_DOC_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);
            return await _repository.ExecuteStoredProcedureAsync<Int64>(RegId, "S_GENERATE_NUMBER_ID", parameters, "@OUTPUT_DOC_ID");
        }

        //Document Number Generation
        public async Task<string> GenrateDocumentNumber(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, DateTime AccountDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inCompanyId", CompanyId, DbType.Int16);
            parameters.Add("@inModuleId", ModuleId, DbType.Int16);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            parameters.Add("@inTrxDate", AccountDate, DbType.Date);
            parameters.Add("@OUTPUT_DOC_NO", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
            return await _repository.ExecuteStoredProcedureAsync<string>(RegId, "S_GENERATE_NUMBER", parameters, "@OUTPUT_DOC_NO");
        }
    }
}