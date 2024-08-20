using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountSetupService
    {
        public Task<AccountSetupViewModelCount> GetAccountSetupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_AccountSetup> GetAccountSetupByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup M_AccountSetup, Int32 UserId);

        public Task<SqlResponce> UpdateAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup M_AccountSetup, Int32 UserId);

        public Task<SqlResponce> DeleteAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup M_AccountSetup, Int32 UserId);
    }
}