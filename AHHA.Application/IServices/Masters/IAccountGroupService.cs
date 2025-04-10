﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountGroupService
    {
        public Task<AccountGroupViewModelCount> GetAccountGroupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<AccountGroupViewModel> GetAccountGroupByIdAsync(string RegId, Int16 CompanyId, Int16 AccGroupId, Int16 UserId);

        public Task<SqlResponse> SaveAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup m_AccountGroup, Int16 UserId);

        public Task<SqlResponse> DeleteAccountGroupAsync(string RegId, Int16 CompanyId, AccountGroupViewModel m_AccountGroup, Int16 UserId);
    }
}