using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerContactService
    {
        public Task<CustomerContactViewModelCount> GetCustomerContactListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_CustomerContact> GetCustomerContactByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerContactAsync(Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerContactAsync(Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerContactAsync(Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
    }
}
