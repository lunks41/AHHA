﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserRightsService
    {
        public Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(string RegId, Int16 CompanyId, Int16 Userid, Int16 UserId);

        public Task<SqlResponce> UpsertUserRightsAsync(string RegId, Int16 CompanyId, AdmUserRights admUserRights, Int16 UserId);
    }
}