using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBankContactService
    {
        public Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 UserId);

        public Task<BankContactViewModel> GetBankContactByIdAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 ContactId, Int16 UserId);

        public Task<SqlResponce> SaveBankContactAsync(string RegId, Int16 CompanyId, M_BankContact m_BankContact, Int16 UserId);

        public Task<SqlResponce> DeleteBankContactAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 ContactId, Int16 UserId);
    }
}