﻿using AHHA.Core.Models.Account;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAROutstandTransactionListAsync(string RegId, Int16 CompanyId, GetTransactionViewModel getTransactionViewModel, Int16 UserId);
    }
}