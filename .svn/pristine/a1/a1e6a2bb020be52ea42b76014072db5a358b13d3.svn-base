using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;

namespace AHHA.Application.IServices.Setting
{
    public interface IDynamicLookupService
    {
        public Task<DynamicLookupViewModel> GetDynamicLookupAsync(string RegId, Int16 CompanyId, Int16 UserId);

        public Task<SqlResponce> UpsertDynamicLookupAsync(string RegId, Int16 CompanyId, S_DynamicLookup s_DynamicLookup, Int16 UserId);
    }
}