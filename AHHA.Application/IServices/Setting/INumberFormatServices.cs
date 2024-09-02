using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Masters;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface INumberFormatServices
    {
        public Task<NumberSettingViewModelCount> GetNumberFormatListAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<S_NumberFormat> GetNumberFormatByIdAsync(string RegId, Int16 CompanyId, Int32 NumberId, Int16 UserId);

        public Task<S_NumberFormatDt> GetNumberFormatByYearAsync(string RegId, Int16 CompanyId, Int32 NumberId, Int32 NumYear, Int16 UserId);

        public Task<SqlResponce> AddNumberFormatAsync(string RegId, Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId);

        public Task<SqlResponce> UpdateNumberFormatAsync(string RegId, Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId);
    }
}