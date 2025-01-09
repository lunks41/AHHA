﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IChartOfAccountService
    {
        public Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_ChartOfAccount> GetChartOfAccountByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId);

        public Task<SqlResponse> SaveChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount M_ChartOfAccount, Int16 UserId);

        public Task<SqlResponse> DeleteChartOfAccountAsync(string RegId, Int16 CompanyId, Int16 GlId, Int16 UserId);
    }
}