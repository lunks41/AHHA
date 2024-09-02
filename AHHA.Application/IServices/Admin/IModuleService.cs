﻿using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface IModuleService
    {
        public Task<IEnumerable<UserModuleViewModel>> GetUsersModulesAsync(string RegId, Int16 CompanyId, Int16 UserId);
    }
}