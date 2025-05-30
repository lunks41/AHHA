﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBankService
    {
        public Task<BankViewModelCount> GetBankListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<BankViewModel> GetBankByIdAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 UserId);

        public Task<BankViewModel> GetBankAsync(string RegId, Int16 CompanyId, Int32 BankId, string BankCode, string BankName, Int16 UserId);

        public Task<SqlResponse> SaveBankAsync(string RegId, Int16 CompanyId, M_Bank M_Bank, Int16 UserId);

        public Task<SqlResponse> DeleteBankAsync(string RegId, Int16 CompanyId, Int16 BankId, Int16 UserId);
    }
}