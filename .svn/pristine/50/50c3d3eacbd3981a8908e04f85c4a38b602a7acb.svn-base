using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPAdjustmentService
    {
        public Task<APAdjustmentViewModelCount> GetAPAdjustmentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<APAdjustmentViewModel> GetAPAdjustmentByIdAsync(string RegId, Int16 CompanyId, Int64 AdjustmentId, string AdjustmentNo, Int16 UserId);

        public Task<SqlResponce> SaveAPAdjustmentAsync(string RegId, Int16 CompanyId, ApAdjustmentHd APAdjustmentHd, List<ApAdjustmentDt> APAdjustmentDts, Int16 UserId);

        public Task<SqlResponce> DeleteAPAdjustmentAsync(string RegId, Int16 CompanyId, Int64 AdjustmentId,string AdjustmentNo, string CanacelRemarks, Int16 UserId);
    }
}