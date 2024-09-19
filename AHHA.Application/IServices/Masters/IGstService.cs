﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Gst> GetGstByIdAsync(string RegId, Int16 CompanyId, Int16 GstId, Int16 UserId);

        public Task<SqlResponce> SaveGstAsync(string RegId, Int16 CompanyId, M_Gst m_GstDt, Int16 UserId);

        public Task<SqlResponce> DeleteGstAsync(string RegId, Int16 CompanyId, M_Gst m_GstDt, Int16 UserId);

        public Task<GstDtViewModelCount> GetGstDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<GstDtViewModel> GetGstDtByIdAsync(string RegId, Int16 CompanyId, Int16 GstDtId, DateTime ValidFrom, Int16 UserId);

        public Task<SqlResponce> SaveGstDtAsync(string RegId, Int16 CompanyId, M_GstDt m_GstDt, Int16 UserId);

        public Task<SqlResponce> DeleteGstDtAsync(string RegId, Int16 CompanyId, M_GstDt m_GstDt, Int16 UserId);
    }
}