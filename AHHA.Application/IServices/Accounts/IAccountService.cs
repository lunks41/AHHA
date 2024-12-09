﻿namespace AHHA.Application.IServices.Accounts
{
    public interface IAccountService
    {
        public Task<Int64> GenrateDocumentId(string RegId, Int16 ModuleId, Int16 TransactionId);

        public Task<string> GenrateDocumentNumber(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, DateTime AccountDate);

        public Task<dynamic> GetARAPPaymentHistoryListAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        //public Task<dynamic> GetHistoryVersionListAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        public Task<dynamic> GetGLPostingHistoryListAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);
    }
}