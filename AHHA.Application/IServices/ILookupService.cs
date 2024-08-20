using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices
{
    public interface ILookupService
    {
        public Task<IEnumerable<CountryLookupViewModel>> GetCountryLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<VesselLookupViewModel>> GetVesselLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<BargeLookupViewModel>> GetBargeLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId);
    }
}