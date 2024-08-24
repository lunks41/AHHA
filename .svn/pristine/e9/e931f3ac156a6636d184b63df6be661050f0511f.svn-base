using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerCreditLimitService
    {
        public Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddCustomerCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);

        public Task<SqlResponce> UpdateCustomerCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);

        public Task<SqlResponce> DeleteCustomerCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);
    }
}