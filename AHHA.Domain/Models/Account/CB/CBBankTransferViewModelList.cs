using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.CB
{
    public class CBBankTransferViewModelList
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CBBankTransferViewModel> data { get; set; }
    }
}