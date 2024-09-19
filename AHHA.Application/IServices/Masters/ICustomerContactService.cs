﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerContactService
    {
        public Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 UserId);

        public Task<CustomerContactViewModel> GetCustomerContactByIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int32 ContactId, Int16 UserId);

        public Task<SqlResponce> AddCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact M_CustomerContact, Int16 UserId);

        public Task<SqlResponce> UpdateCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact M_CustomerContact, Int16 UserId);

        public Task<SqlResponce> DeleteCustomerContactAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int32 ContactId, Int16 UserId);
    }
}