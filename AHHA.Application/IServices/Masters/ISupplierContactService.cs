using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierContactService
    {
        public Task<SupplierContactViewModelCount> GetSupplierContactListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_SupplierContact> GetSupplierContactByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
        public Task<SqlResponce> UpdateSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
        public Task<SqlResponce> DeleteSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int32 UserId);
    }
}
