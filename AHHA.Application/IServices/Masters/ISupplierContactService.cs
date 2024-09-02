﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierContactService
    {
        public Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId);

        public Task<SupplierContactViewModelCount> GetSupplierContactListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_SupplierContact> GetSupplierContactByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int16 UserId);

        public Task<SqlResponce> AddSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int16 UserId);

        public Task<SqlResponce> UpdateSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int16 UserId);

        public Task<SqlResponce> DeleteSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact M_SupplierContact, Int16 UserId);
    }
}