using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserGroupService
    {
        public Task<UserGroupViewModelCount> GetUserGroupListAsync(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, int UserId);

        public Task<AdmUserGroup> GetUserGroupByIdAsync(string RegId, short CompanyId, short COACategoryId, int UserId);

        public Task<SqlResponce> AddUserGroupAsync(string RegId, short CompanyId, AdmUserGroup admUserGroup, int UserId);

        public Task<SqlResponce> UpdateUserGroupAsync(string RegId, short CompanyId, AdmUserGroup admUserGroup, int UserId);

        public Task<SqlResponce> DeleteUserGroupAsync(string RegId, short CompanyId, AdmUserGroup admUserGroup, int UserId);
    }
}