﻿using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices
{
    public interface IBaseService
    {
        UserGroupRightsViewModel ValidateScreen(string RegId, Int16 companyId, Int16 ModuleId, Int16 TransactionId, Int32 userId);
    }
}