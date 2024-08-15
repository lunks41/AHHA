using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerCreditLimitService
    {
        public Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int32 UserId);
    }
}
