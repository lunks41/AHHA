using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerAddressService
    {
        public Task<CustomerAddressViewModelCount> GetCustomerAddressListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_CustomerAddress> GetCustomerAddressByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
    }
}
