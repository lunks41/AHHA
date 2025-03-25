﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerCreditLimitService
    {
        public Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 UserId);

        //public Task<SqlResponse> SaveCustomerCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int16 UserId);

        public Task<SqlResponse> DeleteCustomerCreditLimitAsync(string RegId, Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int16 UserId);
    }
}