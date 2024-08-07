using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface ITransactionService
    {
        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAsync(Int16 CompanyId, Int16 ModuleId, Int32 UserId);

        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsync(Int16 CompanyId, Int32 UserId);
    }
}