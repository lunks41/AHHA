using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Application.IServices.Setting
{
    public interface IDecSettingsService
    {
        public Task<S_DecSettings> GetDecSettingAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponce> UpsertDecSettingAsync(string RegId, Int16 CompanyId, S_DecSettings s_DecSettings, Int16 UserId);
    }
}