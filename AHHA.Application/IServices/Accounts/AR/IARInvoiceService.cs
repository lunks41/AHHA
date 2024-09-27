﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARInvoiceService
    {
        public Task<ARInvoiceViewModelCount> GetARInvoiceListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<ARInvoiceViewModel> GetARInvoiceByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponce> SaveARInvoiceAsync(string RegId, Int16 CompanyId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt, Int16 UserId);

        public Task<SqlResponce> DeleteARInvoiceAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);
    }
}