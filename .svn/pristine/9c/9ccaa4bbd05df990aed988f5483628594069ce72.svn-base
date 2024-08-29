using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserGroupRightsService
    {
        public Task<UserGroupRightsViewModelCount> GetUserGroupRights(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, int UserId);

        public Task<SqlResponce> UpsetUserGroupRights(string RegId, short CompanyId, AdmUserGroupRights admUserGroupRights, int UserId);
    }
}