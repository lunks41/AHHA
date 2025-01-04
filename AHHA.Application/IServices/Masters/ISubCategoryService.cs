﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISubCategoryService
    {
        public Task<SubCategoryViewModelCount> GetSubCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_SubCategory> GetSubCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 SubCategoryId, Int16 UserId);

        public Task<SqlResponse> SaveSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory m_SubCategory, Int16 UserId);

        public Task<SqlResponse> AddSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory m_SubCategory, Int16 UserId);

        public Task<SqlResponse> UpdateSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory m_SubCategory, Int16 UserId);

        public Task<SqlResponse> DeleteSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory m_SubCategory, Int16 UserId);
    }
}