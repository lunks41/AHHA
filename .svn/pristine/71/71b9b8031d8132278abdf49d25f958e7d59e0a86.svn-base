using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARDocSetOffService
    {
        public Task<ARDocSetOffViewModelCount> GetARDocSetOffListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARDocSetOffViewModel> GetARDocSetOffByIdAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);

        public Task<SqlResponse> SaveARDocSetOffAsync(string RegId, Int16 CompanyId, ArDocSetOffHd ARDocSetOffHd, List<ArDocSetOffDt> ARDocSetOffDt, Int16 UserId);

        public Task<SqlResponse> DeleteARDocSetOffAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARDocSetOffViewModel>> GetHistoryARDocSetOffByIdAsync(string RegId, Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);
    }
}