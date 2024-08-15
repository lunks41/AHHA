using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerService
    {
        public Task<CustomerViewModelCount> GetCustomerListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Customer> GetCustomerByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerAsync(Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerAsync(Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerAsync(Int16 CompanyId, M_Customer M_Customer, Int32 UserId);
    }
}
