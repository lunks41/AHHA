﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBPettyCashService
    {
        public Task<CBPettyCashViewModel> GetCBPettyCashListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBPettyCashHdViewModel> GetCBPettyCashByIdNoAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveCBPettyCashAsync(string RegId, Int16 CompanyId, CBPettyCashHd cBPettyCashHd, List<CBPettyCashDt> cBPettyCashDts, Int16 UserId);

        public Task<SqlResponse> DeleteCBPettyCashAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBPettyCashViewModel>> GetHistoryCBPettyCashByIdAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}