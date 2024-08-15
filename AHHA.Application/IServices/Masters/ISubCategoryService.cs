using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISubCategoryService
    {
        public Task<SubCategoryViewModelCount> GetSubCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_SubCategory> GetSubCategoryByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddSubCategoryAsync(Int16 CompanyId, M_SubCategory M_SubCategory, Int32 UserId);
        public Task<SqlResponce> UpdateSubCategoryAsync(Int16 CompanyId, M_SubCategory M_SubCategory, Int32 UserId);
        public Task<SqlResponce> DeleteSubCategoryAsync(Int16 CompanyId, M_SubCategory M_SubCategory, Int32 UserId);
    }
}
