using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerAddressService
    {
        public Task<CustomerAddressViewModelCount> GetCustomerAddressListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_CustomerAddress> GetCustomerAddressByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCustomerAddressAsync(Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
        public Task<SqlResponce> UpdateCustomerAddressAsync(Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
        public Task<SqlResponce> DeleteCustomerAddressAsync(Int16 CompanyId, M_CustomerAddress M_CustomerAddress, Int32 UserId);
    }
}
