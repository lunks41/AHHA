﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IUserSettingService
    {
        public Task<UserSettingViewModel> GetUserSettingAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponse> SaveUserSettingAsync(string RegId, Int16 CompanyId, S_UserSettings S_UserSettings, Int16 UserId);
    }
}