using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierContactService
    {
        public Task<SupplierContactViewModelCount> GetSupplierContactListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_SupplierContact> GetSupplierContactByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSupplierContactAsync(Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
        public Task<SqlResponce> UpdateSupplierContactAsync(Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
        public Task<SqlResponce> DeleteSupplierContactAsync(Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
    }
}
