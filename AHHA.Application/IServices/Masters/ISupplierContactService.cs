﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierContactService
    {
        public Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId);

        public Task<SupplierContactViewModel> GetSupplierContactByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 ContactId, Int16 UserId);

        public Task<SqlResponce> SaveSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int16 UserId);

        public Task<SqlResponce> DeleteSupplierContactAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 ContactId, Int16 UserId);
    }
}