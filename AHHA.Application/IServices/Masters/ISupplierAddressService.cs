using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierAddressService
    {
        public Task<SupplierAddressViewModelCount> GetSupplierAddressListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_SupplierAddress> GetSupplierAddressByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress M_SupplierAddress, Int32 UserId);
        public Task<SqlResponce> UpdateSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress M_SupplierAddress, Int32 UserId);
        public Task<SqlResponce> DeleteSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress M_SupplierAddress, Int32 UserId);
    }
}
