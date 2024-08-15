using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierService
    {
        public Task<SupplierViewModelCount> GetSupplierListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Supplier> GetSupplierByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSupplierAsync(Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
        public Task<SqlResponce> UpdateSupplierAsync(Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
        public Task<SqlResponce> DeleteSupplierAsync(Int16 CompanyId, M_Supplier M_Supplier, Int32 UserId);
    }
}
