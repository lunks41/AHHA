﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARRefundService
    {
        public Task<ARRefundViewModelCount> GetARRefundListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDtae, Int16 UserId);

        public Task<ARRefundViewModel> GetARRefundByIdAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);

        public Task<SqlResponse> SaveARRefundAsync(string RegId, Int16 CompanyId, ArRefundHd ARRefundHd, List<ArRefundDt> ARRefundDt, Int16 UserId);

        public Task<SqlResponse> DeleteARRefundAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARRefundViewModel>> GetHistoryARRefundByIdAsync(string RegId, Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);
    }
}