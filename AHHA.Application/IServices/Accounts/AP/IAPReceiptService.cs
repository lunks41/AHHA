using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPPaymentService
    {
        public Task<APPaymentViewModelCount> GetAPPaymentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APPaymentViewModel> GetAPPaymentByIdAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveAPPaymentAsync(string RegId, Int16 CompanyId, ApPaymentHd APPaymentHd, List<ApPaymentDt> APPaymentDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPPaymentAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APPaymentViewModel>> GetHistoryAPPaymentByIdAsync(string RegId, Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}