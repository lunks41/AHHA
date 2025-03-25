using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Models.Account.GL;

namespace AHHA.Application.IServices.Accounts.GL
{
    public interface IGLContraService
    {
        public Task<GLContraViewModel> GetGLContraListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<GLContraHdViewModel> GetGLContraByIdNoAsync(string RegId, Int16 CompanyId, Int64 ContraId, string ContraNo, Int16 UserId);

        public Task<SqlResponse> SaveGLContraAsync(string RegId, Int16 CompanyId, GLContraHd GLContraHd, List<GLContraDt> GLContraDts, Int16 UserId);

        public Task<SqlResponse> DeleteGLContraAsync(string RegId, Int16 CompanyId, Int64 ContraId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<GLContraHdViewModel>> GetHistoryGLContraByIdAsync(string RegId, Int16 CompanyId, Int64 ContraId, string ContraNo, Int16 UserId);
    }
}