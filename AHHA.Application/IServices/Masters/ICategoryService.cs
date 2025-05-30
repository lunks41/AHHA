﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICategoryService
    {
        public Task<CategoryViewModelCount> GetCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Category> GetCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId);

        public Task<SqlResponse> SaveCategoryAsync(string RegId, Int16 CompanyId, M_Category M_Category, Int16 UserId);

        public Task<SqlResponse> DeleteCategoryAsync(string RegId, Int16 CompanyId, M_Category M_Category, Int16 UserId);
    }
}