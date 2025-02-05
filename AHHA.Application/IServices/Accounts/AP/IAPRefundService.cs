using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPRefundService
    {
        public Task<APRefundViewModelCount> GetAPRefundListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APRefundViewModel> GetAPRefundByIdAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);

        public Task<SqlResponse> SaveAPRefundAsync(string RegId, Int16 CompanyId, ApRefundHd APRefundHd, List<ApRefundDt> APRefundDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPRefundAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APRefundViewModel>> GetHistoryAPRefundByIdAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);
    }
}