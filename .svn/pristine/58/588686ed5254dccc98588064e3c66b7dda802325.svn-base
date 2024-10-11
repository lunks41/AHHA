using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Application.IServices.Accounts.AP;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Account;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Accounts.AP
{
    public sealed class APTransactionService : IAPTransactionService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public APTransactionService(IRepository<dynamic> repository, ApplicationDbContext context, IAccountService accountService)
        {
            _repository = repository;
            _context = context;
            _accountService = accountService;
        }

        public async Task<IEnumerable<GetTransactionViewModel>> GetAPTransactionListAsync(string RegId, Int16 CompanyId, Int16 CurrencyId, Int32 SupplierId, Int16 UserId)
        {
            try
            {
                var productDetails = await _repository.GetQueryAsync<GetTransactionViewModel>(RegId, $"exec Adm_GetUserTransactions {CompanyId},{SupplierId},{CurrencyId}");

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AP,
                    TransactionId = (short)E_AP.Invoice,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "APTransaction",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}