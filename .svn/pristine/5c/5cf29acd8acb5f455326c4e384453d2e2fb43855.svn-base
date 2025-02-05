using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPDocSetOffService
    {
        public Task<APDocSetOffViewModelCount> GetAPDocSetOffListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APDocSetOffViewModel> GetAPDocSetOffByIdAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);

        public Task<SqlResponse> SaveAPDocSetOffAsync(string RegId, Int16 CompanyId, ApDocSetOffHd APDocSetOffHd, List<ApDocSetOffDt> APDocSetOffDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPDocSetOffAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APDocSetOffViewModel>> GetHistoryAPDocSetOffByIdAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);
    }
}