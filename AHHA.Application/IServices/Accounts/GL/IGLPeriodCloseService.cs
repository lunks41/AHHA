﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Models.Account.GL;

namespace AHHA.Application.IServices.Accounts.GL
{
    public interface IGLPeriodCloseService
    {
        public Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(string RegId, Int16 CompanyId, Int32 FinYear, Int16 UserId);

        public Task<SqlResponse> SaveGLPeriodCloseAsync(string RegId, Int16 CompanyId, List<GLPeriodClose> s_MandatoryFields, Int16 UserId);

        public Task<SqlResponse> SaveGLPeriodCloseAsyncV1(string RegId, Int16 CompanyId, PeriodCloseViewModel periodCloseViewModel, Int16 UserId);

        public Task<SqlResponse> SaveNewPeriodCloseAsync(string RegId, Int16 CompanyId, NewPeriodCloseViewModel newPeriodCloseViewModel, Int16 UserId);
        public Task<SqlResponse> DeletePeriodClose(string RegId, Int16 CompanyId, Int16 FinYear, Int16 UserId);
    }
}