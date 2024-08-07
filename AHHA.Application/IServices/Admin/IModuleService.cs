using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface IModuleService
    {
        public Task<IEnumerable<AdmModule>> GetUsersModulesAsync(Int16 CompanyId, Int32 UserId);
    }
}
