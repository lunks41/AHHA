﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IOrderTypeCategoryService
    {
        public Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 OrderTypeCategoryId, Int16 UserId);

        public Task<SqlResponse> SaveOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory m_OrderTypeCategory, Int16 UserId);

        public Task<SqlResponse> DeleteOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory m_OrderTypeCategory, Int16 UserId);
    }
}