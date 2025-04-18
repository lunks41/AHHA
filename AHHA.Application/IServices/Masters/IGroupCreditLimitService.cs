﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimitService
    {
        public Task<GroupCreditLimitViewModelCount> GetGroupCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_GroupCreditLimit> GetGroupCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId);

        public Task<SqlResponse> SaveGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);

        public Task<SqlResponse> DeleteGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);
    }
}