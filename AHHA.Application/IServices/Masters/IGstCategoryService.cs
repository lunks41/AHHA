﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstCategoryService
    {
        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_GstCategory> GetGstCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 GstCategoryId, Int16 UserId);

        public Task<SqlResponse> SaveGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory m_GstCategory, Int16 UserId);

        public Task<SqlResponse> DeleteGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory m_GstCategory, Int16 UserId);
    }
}