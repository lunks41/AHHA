using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstCategoryService
    {
        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_GstCategory> GetGstCategoryByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddGstCategoryAsync(Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);
        public Task<SqlResponce> UpdateGstCategoryAsync(Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);
        public Task<SqlResponce> DeleteGstCategoryAsync(Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);
    }
}
