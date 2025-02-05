﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARReceiptService
    {
        public Task<ARReceiptViewModelCount> GetARReceiptListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARReceiptViewModel> GetARReceiptByIdAsync(string RegId, Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, Int16 UserId);

        public Task<SqlResponse> SaveARReceiptAsync(string RegId, Int16 CompanyId, ArReceiptHd ARReceiptHd, List<ArReceiptDt> ARReceiptDt, Int16 UserId);

        public Task<SqlResponse> DeleteARReceiptAsync(string RegId, Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARReceiptViewModel>> GetHistoryARReceiptByIdAsync(string RegId, Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, Int16 UserId);
    }
}