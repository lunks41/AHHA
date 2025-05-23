﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountSetupCategoryService
    {
        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupCategoryId, Int16 UserId);

        public Task<SqlResponse> SaveAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory m_AccountSetupCategory, Int16 UserId);

        public Task<SqlResponse> DeleteAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory m_AccountSetupCategory, Int16 UserId);
    }
}