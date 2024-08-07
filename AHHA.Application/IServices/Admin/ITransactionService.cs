using AHHA.Core.Entities.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface ITransactionService
    {
        public Task<IEnumerable<AdmTransaction>> GetUsersTransactionsAsync(Int16 CompanyId, Int16 ModuleId, Int32 UserId);

        public Task<IEnumerable<AdmTransaction>> GetUsersTransactionsAllAsync(Int16 CompanyId, Int16 ModuleId, Int32 UserId);
    }
}