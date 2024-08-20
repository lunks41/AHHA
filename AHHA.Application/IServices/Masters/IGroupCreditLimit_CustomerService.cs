﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimit_CustomerService
    {
        public Task<GroupCreditLimt_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_GroupCreditLimt_Customer> GetGroupCreditLimit_CustomerByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt_Customer M_GroupCreditLimt_Customer, Int32 UserId);

        public Task<SqlResponce> UpdateGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt_Customer M_GroupCreditLimt_Customer, Int32 UserId);

        public Task<SqlResponce> DeleteGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt_Customer M_GroupCreditLimt_Customer, Int32 UserId);
    }
}