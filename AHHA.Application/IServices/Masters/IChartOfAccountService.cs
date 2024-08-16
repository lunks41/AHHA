using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IChartOfAccountService
    {
        public Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_ChartOfAccount> GetChartOfAccountByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount M_ChartOfAccount, Int32 UserId);
        public Task<SqlResponce> UpdateChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount M_ChartOfAccount, Int32 UserId);
        public Task<SqlResponce> DeleteChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount M_ChartOfAccount, Int32 UserId);
    }
}
