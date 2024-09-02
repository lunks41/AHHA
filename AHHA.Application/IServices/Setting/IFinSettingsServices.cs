using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.IServices.Setting
{
    public interface IFinSettingsService
    {
        public Task<S_FinSettings> GetFinSettingAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponce> UpsertFinSettingAsync(string RegId, Int16 CompanyId, S_FinSettings s_FinSettings, Int16 UserId);
    }
}