using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IPaymentTypeService
    {
        public Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_PaymentType> GetPaymentTypeByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddPaymentTypeAsync(Int16 CompanyId, M_PaymentType M_PaymentType, Int32 UserId);
        public Task<SqlResponce> UpdatePaymentTypeAsync(Int16 CompanyId, M_PaymentType M_PaymentType, Int32 UserId);
        public Task<SqlResponce> DeletePaymentTypeAsync(Int16 CompanyId, M_PaymentType M_PaymentType, Int32 UserId);
    }
}
