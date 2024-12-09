using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Models.Account.GL;

namespace AHHA.Application.IServices.Accounts.GL
{
    public interface IGLPeriodCloseService
    {
        public Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(string RegId, Int16 CompanyId, Int32 FinYear, Int16 UserId);

        public Task<SqlResponce> SaveGLPeriodCloseAsync(string RegId, Int16 CompanyId, PeriodCloseViewModel periodCloseViewModel, Int16 UserId);
    }
}