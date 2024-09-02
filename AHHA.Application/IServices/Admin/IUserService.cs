﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserService
    {
        public Task<UserViewModelCount> GetUserListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId);

        public Task<AdmUser> GetUserByIdAsync(string RegId, Int16 CompanyId, Int16 Userid, Int16 UserId);

        public Task<SqlResponce> AddUserAsync(string RegId, Int16 CompanyId, AdmUser admUser, Int16 UserId);

        public Task<SqlResponce> UpdateUserAsync(string RegId, Int16 CompanyId, AdmUser admUser, Int16 UserId);

        public Task<SqlResponce> DeleteUserAsync(string RegId, Int16 CompanyId, AdmUser admUser, Int16 UserId);

        public Task<SqlResponce> ResetPasswordAsync(string RegId, Int16 CompanyId, AdmUser admUser, Int16 UserId);
    }
}