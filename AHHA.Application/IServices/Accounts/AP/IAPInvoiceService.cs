using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPInvoiceService
    {
        public Task<APInvoiceViewModelCount> GetAPInvoiceListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APInvoiceViewModel> GetAPInvoiceByIdNoAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveAPInvoiceAsync(string RegId, Int16 CompanyId, ApInvoiceHd APInvoiceHd, List<ApInvoiceDt> APInvoiceDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPInvoiceAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APInvoiceViewModel>> GetHistoryAPInvoiceByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);
    }
}