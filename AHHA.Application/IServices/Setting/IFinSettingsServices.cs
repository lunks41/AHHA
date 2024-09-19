﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IFinSettingsService
    {
        public Task<FinSettingViewModel> GetFinSettingAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponce> UpsertFinSettingAsync(string RegId, Int16 CompanyId, S_FinSettings s_FinSettings, Int16 UserId);
    }
}