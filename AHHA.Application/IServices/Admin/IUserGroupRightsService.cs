﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserGroupRightsService
    {
        public Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(string RegId, Int16 CompanyId, Int16 UserGroupId, Int16 UserId);

        public Task<SqlResponse> SaveUserGroupRightsAsync(string RegId, Int16 CompanyId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId, Int16 UserId);
        public Task<SqlResponse> CloneUserGroupRightsAsync(string RegId, Int16 CompanyId, Int16 FromUserGroupId, Int16 ToUserGroupId, Int16 UserId);
    }
}