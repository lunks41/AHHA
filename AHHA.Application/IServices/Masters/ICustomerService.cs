using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerService
    {
        public Task<CustomerViewModelCount> GetCustomerListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Customer> GetCustomerByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerAsync(string RegId, Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerAsync(string RegId, Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerAsync(string RegId, Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
    }
}
