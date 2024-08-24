﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerGroupCreditLimitService
    {
        public Task<CustomerGroupCreditLimitViewModelCount> GetCustomerGroupCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_CustomerGroupCreditLimit> GetCustomerGroupCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddCustomerGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerGroupCreditLimit M_CustomerGroupCreditLimit, Int32 UserId);

        public Task<SqlResponce> UpdateCustomerGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerGroupCreditLimit M_CustomerGroupCreditLimit, Int32 UserId);

        public Task<SqlResponce> DeleteCustomerGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerGroupCreditLimit M_CustomerGroupCreditLimit, Int32 UserId);
    }
}