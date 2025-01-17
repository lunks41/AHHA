﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierService
    {
        public Task<SupplierViewModelCount> GetSupplierListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<SupplierViewModel> GetSupplierAsync(string RegId, Int16 CompanyId, Int32 SupplierId, string SupplierCode, string SupplierName, Int16 UserId);

        public Task<SqlResponse> SaveSupplierAsync(string RegId, Int16 CompanyId, M_Supplier M_Supplier, Int16 UserId);

        public Task<SqlResponse> DeleteSupplierAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId);
    }
}