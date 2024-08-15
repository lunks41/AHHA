using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICategoryService
    {
        public Task<CategoryViewModelCount> GetCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Category> GetCategoryByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCategoryAsync(Int16 CompanyId, M_Category M_Category, Int32 UserId);
        public Task<SqlResponce> UpdateCategoryAsync(Int16 CompanyId, M_Category M_Category, Int32 UserId);
        public Task<SqlResponce> DeleteCategoryAsync(Int16 CompanyId, M_Category M_Category, Int32 UserId);
    }
}
