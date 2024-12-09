using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Models.Account.GL;

namespace AHHA.Application.IServices.Accounts.GL
{
    public interface IGLOpeningBalanceService
    {
        public Task<IEnumerable<GLOpeningBalanceViewModel>> GetGLOpeningBalanceListAsync(string RegId, Int16 CompanyId, Int32 FinYear, Int16 UserId);

        //public Task<SqlResponce> SaveGLOpeningBalanceAsync(string RegId, Int16 CompanyId, GLOpeningBalanceViewModel OpeningBalanceViewModel, Int16 UserId);
    }
}