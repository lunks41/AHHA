﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierAddressService
    {
        public Task<IEnumerable<SupplierAddressViewModel>> GetSupplierAddressBySupplierIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId);

        public Task<SupplierAddressViewModel> GetSupplierAddressByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 AddressId, Int16 UserId);

        public Task<SqlResponse> SaveSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress m_SupplierAddress, Int16 UserId);

        public Task<SqlResponse> DeleteSupplierAddressAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 AddressId, Int16 UserId);
    }
}