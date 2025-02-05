using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBGenPaymentService
    {
        public Task<CBGenPaymentViewModel> GetCBGenPaymentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBGenPaymentHdViewModel> GetCBGenPaymentByIdNoAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveCBGenPaymentAsync(string RegId, Int16 CompanyId, CBGenPaymentHd cBGenPaymentHd, List<CBGenPaymentDt> cBGenPaymentDts, Int16 UserId);

        public Task<SqlResponse> DeleteCBGenPaymentAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBGenPaymentHdViewModel>> GetHistoryCBGenPaymentByIdAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}