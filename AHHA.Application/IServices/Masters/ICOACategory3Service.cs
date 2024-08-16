﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICOACategory3Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory3ListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_COACategory3> GetCOACategory3ByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddCOACategory3Async(string RegId, Int16 CompanyId, M_COACategory3 M_COACategory3, Int32 UserId);
        public Task<SqlResponce> UpdateCOACategory3Async(string RegId, Int16 CompanyId, M_COACategory3 M_COACategory3, Int32 UserId);
        public Task<SqlResponce> DeleteCOACategory3Async(string RegId, Int16 CompanyId, M_COACategory3 M_COACategory3, Int32 UserId);
    }
}
