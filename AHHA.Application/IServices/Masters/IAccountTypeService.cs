﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountTypeService
    {
        public Task<AccountTypeViewModelCount> GetAccountTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_AccountType> GetAccountTypeByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType M_AccountType, Int32 UserId);

        public Task<SqlResponce> UpdateAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType M_AccountType, Int32 UserId);

        public Task<SqlResponce> DeleteAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType M_AccountType, Int32 UserId);
    }
}