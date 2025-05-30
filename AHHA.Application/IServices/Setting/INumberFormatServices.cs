﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface INumberFormatServices
    {
        public Task<ModelNameViewModelCount> GetNumberFormatListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<NumberSettingViewModel> GetNumberFormatByIdAsync(string RegId, Int16 CompanyId, Int32 ModuleId, Int32 TransactionId, Int16 UserId);

        public Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(string RegId, Int16 CompanyId, Int32 NumberId, Int32 NumYear, Int16 UserId);

        public Task<SqlResponse> SaveNumberFormatAsync(string RegId, Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId);
    }
}