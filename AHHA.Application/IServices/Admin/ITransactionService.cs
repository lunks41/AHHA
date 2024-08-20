using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface ITransactionService
    {
        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int32 UserId);

        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsync(string RegId, Int16 CompanyId, Int32 UserId);
    }
}