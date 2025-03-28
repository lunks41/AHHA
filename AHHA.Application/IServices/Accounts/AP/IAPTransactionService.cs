﻿using AHHA.Core.Models.Account;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAPOutstandTransactionListAsync(string RegId, Int16 CompanyId, GetTransactionViewModel getTransactionViewModel, Int16 UserId);
    }
}