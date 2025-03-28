﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IAccountSetupService
    {
        #region Header

        public Task<AccountSetupViewModelCount> GetAccountSetupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_AccountSetup> GetAccountSetupByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupId, Int16 UserId);

        public Task<SqlResponse> SaveAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup M_AccountSetup, Int16 UserId);

        public Task<SqlResponse> DeleteAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup M_AccountSetup, Int16 UserId);

        #endregion Header

        #region Details

        public Task<AccountSetupDtViewModelCount> GetAccountSetupDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<AccountSetupDtViewModel> GetAccountSetupDtByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupId, Int16 UserId);

        public Task<SqlResponse> SaveAccountSetupDtAsync(string RegId, Int16 CompanyId, M_AccountSetupDt m_AccountSetupDt, Int16 UserId);

        public Task<SqlResponse> DeleteAccountSetupDtAsync(string RegId, Int16 CompanyId, AccountSetupDtViewModel accountSetupDtViewModel, Int16 UserId);

        #endregion Details
    }
}