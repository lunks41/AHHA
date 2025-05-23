﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IPaymentTypeService
    {
        public Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_PaymentType> GetPaymentTypeByIdAsync(string RegId, Int16 CompanyId, Int16 PaymentTypeId, Int16 UserId);

        public Task<SqlResponse> SavePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType m_PaymentType, Int16 UserId);

        public Task<SqlResponse> DeletePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType m_PaymentType, Int16 UserId);
    }
}