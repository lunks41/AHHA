using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierService
    {
        public Task<SupplierViewModelCount> GetSupplierListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Supplier> GetSupplierByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSupplierAsync(string RegId, Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
        public Task<SqlResponce> UpdateSupplierAsync(string RegId, Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
        public Task<SqlResponce> DeleteSupplierAsync(string RegId, Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
    }
}
