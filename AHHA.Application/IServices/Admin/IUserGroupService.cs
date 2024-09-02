﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserGroupService
    {
        public Task<UserGroupViewModelCount> GetUserGroupListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId);

        public Task<AdmUserGroup> GetUserGroupByIdAsync(string RegId, Int16 CompanyId, short COACategoryId, Int16 UserId);

        public Task<SqlResponce> AddUserGroupAsync(string RegId, Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId);

        public Task<SqlResponce> UpdateUserGroupAsync(string RegId, Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId);

        public Task<SqlResponce> DeleteUserGroupAsync(string RegId, Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId);
    }
}