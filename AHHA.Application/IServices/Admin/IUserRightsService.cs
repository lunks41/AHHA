﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserRightsService
    {
        public Task<SqlResponce> AddUserRightsAsync(string RegId, short CompanyId, AdmUserRights admUserRights, int UserId);

        public Task<SqlResponce> UpdateUserRightsAsync(string RegId, short CompanyId, AdmUserRights admUserRights, int UserId);
    }
}