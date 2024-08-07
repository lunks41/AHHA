using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface IModuleService
    {
        public Task<IEnumerable<UsersModuleViewModel>> GetUsersModulesAsync(Int16 CompanyId, Int32 UserId);
    }
}
