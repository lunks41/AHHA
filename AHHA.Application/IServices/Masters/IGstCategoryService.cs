﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstCategoryService
    {
        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_GstCategory> GetGstCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);

        public Task<SqlResponce> UpdateGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);

        public Task<SqlResponce> DeleteGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory M_GstCategory, Int32 UserId);
    }
}