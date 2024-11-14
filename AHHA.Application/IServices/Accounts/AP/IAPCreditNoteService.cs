using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPCreditNoteService
    {
        public Task<APCreditNoteViewModelCount> GetAPCreditNoteListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<APCreditNoteViewModel> GetAPCreditNoteByIdAsync(string RegId, Int16 CompanyId, Int64 CreditNoteId, string CreditNoteNo, Int16 UserId);

        public Task<SqlResponce> SaveAPCreditNoteAsync(string RegId, Int16 CompanyId, ApCreditNoteHd APCreditNoteHd, List<ApCreditNoteDt> APCreditNoteDts, Int16 UserId);

        public Task<SqlResponce> DeleteAPCreditNoteAsync(string RegId, Int16 CompanyId, Int64 CreditNoteId,string CreditNoteNo, string CanacelRemarks, Int16 UserId);
    }
}