﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ITaxService
    {
        public Task<TaxViewModelCount> GetTaxListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Tax> GetTaxByIdAsync(string RegId, Int16 CompanyId, Int16 TaxId, Int16 UserId);

        public Task<SqlResponse> SaveTaxAsync(string RegId, Int16 CompanyId, M_Tax m_Tax, Int16 UserId);

        public Task<SqlResponse> DeleteTaxAsync(string RegId, Int16 CompanyId, M_Tax m_Tax, Int16 UserId);

        public Task<TaxDtViewModelCount> GetTaxDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<TaxDtViewModel> GetTaxDtByIdAsync(string RegId, Int16 CompanyId, Int16 TaxId, DateTime ValidFrom, Int16 UserId);

        public Task<SqlResponse> SaveTaxDtAsync(string RegId, Int16 CompanyId, M_TaxDt m_TaxDt, Int16 UserId);

        public Task<SqlResponse> DeleteTaxDtAsync(string RegId, Int16 CompanyId, TaxDtViewModel m_TaxDt, Int16 UserId);
    }
}