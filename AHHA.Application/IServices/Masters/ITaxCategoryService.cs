using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ITaxCategoryService
    {
        public Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_TaxCategory> GetTaxCategoryByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddTaxCategoryAsync(Int16 CompanyId, M_TaxCategory M_TaxCategory, Int32 UserId);
        public Task<SqlResponce> UpdateTaxCategoryAsync(Int16 CompanyId, M_TaxCategory M_TaxCategory, Int32 UserId);
        public Task<SqlResponce> DeleteTaxCategoryAsync(Int16 CompanyId, M_TaxCategory M_TaxCategory, Int32 UserId);
    }
}
