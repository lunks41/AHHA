using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBBankReconService
    {
        public Task<CBBankReconViewModel> GetCBBankReconListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBBankReconHdViewModel> GetCBBankReconByIdNoAsync(string RegId, Int16 CompanyId, Int64 ReconId, string ReconNo, Int16 UserId);

        public Task<SqlResponse> SaveCBBankReconAsync(string RegId, Int16 CompanyId, CBBankReconHd CBBankReconHd, List<CBBankReconDt> CBBankReconDt, Int16 UserId);

        public Task<SqlResponse> DeleteCBBankReconAsync(string RegId, Int16 CompanyId, Int64 ReconId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBBankReconHdViewModel>> GetHistoryCBBankReconByIdAsync(string RegId, Int16 CompanyId, Int64 ReconId, string ReconNo, Int16 UserId);
    }
}