﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomerAddressService
    {
        public Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 UserId);

        public Task<CustomerAddressViewModel> GetCustomerAddressByIdAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 AddressId, Int16 UserId);

        public Task<SqlResponse> SaveCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress m_CustomerAddress, Int16 UserId);

        public Task<SqlResponse> DeleteCustomerAddressAsync(string RegId, Int16 CompanyId, Int32 CustomerId, Int16 AddressId, Int16 UserId);
    }
}