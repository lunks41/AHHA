﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerService
    {
        public Task<CustomerViewModelCount> GetCustomerListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<CustomerViewModel> GetCustomerAsync(string RegId, Int16 CompanyId, Int32 CustomerId, string CustomerCode, string CustomerName, Int16 UserId);

        public Task<SqlResponse> SaveCustomerAsync(string RegId, Int16 CompanyId, M_Customer M_Customer, Int16 UserId);

        public Task<SqlResponse> DeleteCustomerAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 UserId);
    }
}