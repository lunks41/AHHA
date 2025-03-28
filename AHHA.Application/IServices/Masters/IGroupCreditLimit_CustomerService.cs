﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimit_CustomerService
    {
        public Task<GroupCreditLimit_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_GroupCreditLimit_Customer> GetGroupCreditLimit_CustomerByIdAsync(string RegId, Int16 CompanyId, Int16 GroupCreditLimitId, Int16 UserId);

        public Task<SqlResponse> SaveGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer, Int16 UserId);

        public Task<SqlResponse> DeleteGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer, Int16 UserId);
    }
}