﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPInvoiceService
    {
        public Task<APInvoiceViewModelCount> GetAPInvoiceListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<APInvoiceViewModel> GetAPInvoiceByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveAPInvoiceAsync(string RegId, Int16 CompanyId, ApInvoiceHd apInvoiceHd, List<ApInvoiceDt> apInvoiceDts, Int16 UserId);

        public Task<SqlResponse> DeleteAPInvoiceAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, string CanacelRemarks, Int16 UserId);
    }
}