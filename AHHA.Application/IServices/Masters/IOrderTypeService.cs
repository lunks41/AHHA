﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IOrderTypeService
    {
        public Task<OrderTypeViewModelCount> GetOrderTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_OrderType> GetOrderTypeByIdAsync(string RegId, Int16 CompanyId, Int16 OrderTypeId, Int16 UserId);

        public Task<SqlResponse> SaveOrderTypeAsync(string RegId, Int16 CompanyId, M_OrderType m_OrderType, Int16 UserId);

        public Task<SqlResponse> DeleteOrderTypeAsync(string RegId, Int16 CompanyId, M_OrderType m_OrderType, Int16 UserId);
    }
}