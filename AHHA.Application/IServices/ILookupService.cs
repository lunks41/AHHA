using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Auth;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices
{
    public interface ILookupService
    {
        public Task<IEnumerable<CountryLookupViewModel>> GetCountryLooupListAsync(Int16 CompanyId, Int32 UserId);
    }
}
