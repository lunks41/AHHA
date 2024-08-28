using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountGroupService
    {
        public Task<AccountGroupViewModelCount> GetAccountGroupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_AccountGroup> GetAccountGroupByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup M_AccountGroup, Int32 UserId);

        public Task<SqlResponce> UpdateAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup M_AccountGroup, Int32 UserId);

        public Task<SqlResponce> DeleteAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup M_AccountGroup, Int32 UserId);
    }
}