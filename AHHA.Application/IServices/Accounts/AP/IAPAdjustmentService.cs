﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPAdjustmentService
    {
        public Task<APAdjustmentViewModelCount> GetAPAdjustmentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APAdjustmentViewModel> GetAPAdjustmentByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveAPAdjustmentAsync(string RegId, Int16 CompanyId, ApAdjustmentHd APAdjustmentHd, List<ApAdjustmentDt> APAdjustmentDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPAdjustmentAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APAdjustmentViewModel>> GetHistoryAPAdjustmentByIdAsync(string RegId, Int16 CompanyId, Int64 AdjustmentId, string AdjustmentNo, Int16 UserId);
    }
}