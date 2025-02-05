using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBGenReceiptService
    {
        public Task<CBGenReceiptViewModel> GetCBGenReceiptListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBGenReceiptHdViewModel> GetCBGenReceiptByIdNoAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveCBGenReceiptAsync(string RegId, Int16 CompanyId, CBGenReceiptHd CBGenReceiptHd, List<CBGenReceiptDt> CBGenReceiptDt, Int16 UserId);

        public Task<SqlResponse> DeleteCBGenReceiptAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBGenReceiptViewModel>> GetHistoryCBGenReceiptByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);
    }
}