using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountSetupCategoryService
    {
        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddAccountSetupCategoryAsync(Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
        public Task<SqlResponce> UpdateAccountSetupCategoryAsync(Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
        public Task<SqlResponce> DeleteAccountSetupCategoryAsync(Int16 CompanyId, M_AccountSetupCategory M_AccountSetupCategory, Int32 UserId);
    }
}
