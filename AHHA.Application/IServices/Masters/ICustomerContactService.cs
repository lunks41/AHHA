using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerContactService
    {
        public Task<CustomerContactViewModelCount> GetCustomerContactListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_CustomerContact> GetCustomerContactByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact M_CustomerContact, Int32 UserId);
    }
}
