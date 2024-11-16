﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBPettyCashService
    {
        public Task<CBPettyCashViewModel> GetCBPettyCashListAsync(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);

        public Task<CBPettyCashHdViewModel> GetCBPettyCashByIdAsync(string RegId, short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponce> SaveCBPettyCashAsync(string RegId, short CompanyId, CBPettyCashHd cBPettyCashHd, List<CBPettyCashDt> cBPettyCashDts, short UserId);

        public Task<SqlResponce> DeleteCBPettyCashAsync(string RegId, short CompanyId, long PaymentId, string PaymentNo, string CanacelRemarks, short UserId);
    }
}