﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ITaxCategoryService
    {
        public Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_TaxCategory> GetTaxCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 TaxCategoryId, Int16 UserId);

        public Task<SqlResponse> SaveTaxCategoryAsync(string RegId, Int16 CompanyId, M_TaxCategory m_TaxCategory, Int16 UserId);

        public Task<SqlResponse> DeleteTaxCategoryAsync(string RegId, Int16 CompanyId, M_TaxCategory m_TaxCategory, Int16 UserId);
    }
}