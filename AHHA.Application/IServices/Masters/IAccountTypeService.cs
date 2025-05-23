﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountTypeService
    {
        public Task<AccountTypeViewModelCount> GetAccountTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_AccountType> GetAccountTypeByIdAsync(string RegId, Int16 CompanyId, Int16 AccTypeId, Int16 UserId);

        public Task<SqlResponse> SaveAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType M_AccountType, Int16 UserId);

        public Task<SqlResponse> DeleteAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType M_AccountType, Int16 UserId);
    }
}