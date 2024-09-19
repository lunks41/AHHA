﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ITaxService
    {
        public Task<TaxViewModelCount> GetTaxListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Tax> GetTaxByIdAsync(string RegId, Int16 CompanyId, Int16 TaxId, Int16 UserId);

        public Task<SqlResponce> SaveTaxAsync(string RegId, Int16 CompanyId, M_Tax m_Tax, Int16 UserId);

        public Task<SqlResponce> DeleteTaxAsync(string RegId, Int16 CompanyId, M_Tax m_Tax, Int16 UserId);

        public Task<TaxDtViewModelCount> GetTaxDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<GstDtViewModel> GetTaxDtByIdAsync(string RegId, Int16 CompanyId, Int16 TaxId, DateTime ValidFrom, Int16 UserId);

        public Task<SqlResponce> SaveTaxDtAsync(string RegId, Int16 CompanyId, M_TaxDt m_TaxDt, Int16 UserId);

        public Task<SqlResponce> DeleteTaxDtAsync(string RegId, Int16 CompanyId, M_TaxDt m_TaxDt, Int16 UserId);
    }
}