﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IOrderTypeCategoryService
    {
        public Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory M_OrderTypeCategory, Int32 UserId);

        public Task<SqlResponce> UpdateOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory M_OrderTypeCategory, Int32 UserId);

        public Task<SqlResponce> DeleteOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory M_OrderTypeCategory, Int32 UserId);
    }
}