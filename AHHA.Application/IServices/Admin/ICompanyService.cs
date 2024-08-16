﻿using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface ICompanyService
    {
        public Task<IEnumerable<CompanyViewModel>> GetUserCompanyListAsync(string RegId,Int32 UserId);
    }
}
