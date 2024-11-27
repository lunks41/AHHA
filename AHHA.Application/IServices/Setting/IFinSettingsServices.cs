﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IFinanceSettingService
    {
        public Task<FinanceSettingViewModel> GetFinSettingAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponce> SaveFinSettingAsync(string RegId, Int16 CompanyId, S_FinSettings s_FinSettings, Int16 UserId);
    }
}