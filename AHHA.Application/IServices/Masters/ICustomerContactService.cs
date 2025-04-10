﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerContactService
    {
        public Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 UserId);

        public Task<CustomerContactViewModel> GetCustomerContactByIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 ContactId, Int16 UserId);

        public Task<SqlResponse> SaveCustomerContactAsync(string RegId, Int16 CompanyId, M_CustomerContact m_CustomerContact, Int16 UserId);

        public Task<SqlResponse> DeleteCustomerContactAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 ContactId, Int16 UserId);
    }
}