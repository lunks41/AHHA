using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Models.Account.GL;

namespace AHHA.Application.IServices.Accounts.GL
{
    public interface IGLJournalService
    {
        public Task<GLJournalViewModel> GetGLJournalListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<GLJournalHdViewModel> GetGLJournalByIdNoAsync(string RegId, Int16 CompanyId, Int64 JournalId, string JournalNo, Int16 UserId);

        public Task<SqlResponse> SaveGLJournalAsync(string RegId, Int16 CompanyId, GLJournalHd GLJournalHd, List<GLJournalDt> GLJournalDts, Int16 UserId);

        public Task<SqlResponse> DeleteGLJournalAsync(string RegId, Int16 CompanyId, Int64 JournalId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<GLJournalHdViewModel>> GetHistoryGLJournalByIdAsync(string RegId, Int16 CompanyId, Int64 JournalId, string JournalNo, Int16 UserId);
    }
}