﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IUomService
    {
        public Task<UomViewModelCount> GetUomListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Uom> GetUomByIdAsync(string RegId, Int16 CompanyId, Int16 UomId, Int16 UserId);

        public Task<SqlResponse> SaveUomAsync(string RegId, Int16 CompanyId, M_Uom m_Uom, Int16 UserId);

        public Task<SqlResponse> DeleteUomAsync(string RegId, Int16 CompanyId, M_Uom m_Uom, Int16 UserId);

        public Task<UomDtViewModelCount> GetUomDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<UomDtViewModel> GetUomDtByIdAsync(string RegId, Int16 CompanyId, Int16 UomId, Int16 PackUomId, Int16 UserId);

        public Task<SqlResponse> SaveUomDtAsync(string RegId, Int16 CompanyId, M_UomDt m_UomDt, Int16 UserId);

        public Task<SqlResponse> DeleteUomDtAsync(string RegId, Int16 CompanyId, UomDtViewModel m_UomDt, Int16 UserId);
    }
}