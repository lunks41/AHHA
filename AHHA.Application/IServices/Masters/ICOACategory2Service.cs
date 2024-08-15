﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICOACategory2Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory2ListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_COACategory2> GetCOACategory2ByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCOACategory2Async(Int16 CompanyId, M_COACategory2 M_COACategory2, Int32 UserId);
        public Task<SqlResponce> UpdateCOACategory2Async(Int16 CompanyId, M_COACategory2 M_COACategory2, Int32 UserId);
        public Task<SqlResponce> DeleteCOACategory2Async(Int16 CompanyId, M_COACategory2 M_COACategory2, Int32 UserId);
    }
}