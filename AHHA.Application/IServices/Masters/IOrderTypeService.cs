using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IOrderTypeService
    {
        public Task<OrderTypeViewModelCount> GetOrderTypeListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_OrderType> GetOrderTypeByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddOrderTypeAsync(Int16 CompanyId, M_OrderType M_OrderType, Int32 UserId);
        public Task<SqlResponce> UpdateOrderTypeAsync(Int16 CompanyId, M_OrderType M_OrderType, Int32 UserId);
        public Task<SqlResponce> DeleteOrderTypeAsync(Int16 CompanyId, M_OrderType M_OrderType, Int32 UserId);
    }
}
