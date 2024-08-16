using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountSetupCategoryService
    {
        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
        public Task<SqlResponce> UpdateAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
        public Task<SqlResponce> DeleteAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
    }
}
