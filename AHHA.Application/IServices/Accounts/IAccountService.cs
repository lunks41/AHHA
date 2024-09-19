﻿namespace AHHA.Application.IServices.Accounts
{
    public interface IAccountService
    {
        public Task<Int64> GenrateDocumentId(string RegId, Int16 ModuleId, Int16 TransactionId);

        public Task<string> GenrateDocumentNumber(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, DateTime AccountDate);
    }
}