using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices
{
    public interface IMasterLookupService
    {
        public Task<IEnumerable<CountryLookupModel>> GetCountryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);

        public Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupListAsync(string RegId, Int16 CompanyId, Int32 UserId);
    }
}