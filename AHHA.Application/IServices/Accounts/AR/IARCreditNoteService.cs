using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARCreditNoteService
    {
        public Task<ARCreditNoteViewModelCount> GetARCreditNoteListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<ARCreditNoteViewModel> GetARCreditNoteByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponce> SaveARCreditNoteAsync(string RegId, Int16 CompanyId, ArCreditNoteHd ARCreditNoteHd, List<ArCreditNoteDt> ARCreditNoteDt, Int16 UserId);

        public Task<SqlResponce> DeleteARCreditNoteAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);
    }
}