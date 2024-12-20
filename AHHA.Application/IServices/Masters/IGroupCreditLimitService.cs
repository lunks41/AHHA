﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimitService
    {
        public Task<GroupCreditLimitViewModelCount> GetGroupCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_GroupCreditLimit> GetGroupCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId);

        public Task<SqlResponce> SaveGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);

        public Task<SqlResponce> AddGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);

        public Task<SqlResponce> UpdateGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);

        public Task<SqlResponce> DeleteGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit M_GroupCreditLimit, Int16 UserId);
    }
}