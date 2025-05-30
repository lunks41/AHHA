﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBargeService
    {
        public Task<BargeViewModelCount> GetBargeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<BargeViewModel> GetBargeByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId);

        public Task<SqlResponse> SaveBargeAsync(string RegId, Int16 CompanyId, M_Barge M_Barge, Int16 UserId);

        public Task<SqlResponse> DeleteBargeAsync(string RegId, Int16 CompanyId, BargeViewModel bargeViewModel, Int16 UserId);
    }
}