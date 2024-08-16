using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBankService
    {
        public Task<BankViewModelCount> GetBankListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Bank> GetBankByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddBankAsync(string RegId, Int16 CompanyId, M_Bank M_Bank, Int32 UserId);
        public Task<SqlResponce> UpdateBankAsync(string RegId, Int16 CompanyId, M_Bank M_Bank, Int32 UserId);
        public Task<SqlResponce> DeleteBankAsync(string RegId, Int16 CompanyId, M_Bank M_Bank, Int32 UserId);
    }
}
