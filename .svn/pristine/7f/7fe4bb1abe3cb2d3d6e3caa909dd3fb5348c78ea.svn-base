using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Models.Account.CB;

namespace AHHA.Application.IServices.Accounts.CB
{
    public interface ICBBankTransferService
    {
        public Task<CBBankTransferViewModelList> GetCBBankTransferListAsync(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);

        public Task<CBBankTransferViewModel> GetCBBankTransferByIdAsync(string RegId, short CompanyId, long TransferId, string TransferNo, short UserId);

        public Task<SqlResponce> SaveCBBankTransferAsync(string RegId, short CompanyId, CBBankTransfer cBBankTransfer, short UserId);

        public Task<SqlResponce> DeleteCBBankTransferAsync(string RegId, short CompanyId, long TransferId, string TransferNo, string CanacelRemarks, short UserId);
    }
}