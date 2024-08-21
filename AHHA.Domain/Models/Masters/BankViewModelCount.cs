using AHHA.Core.Common;

namespace AHHA.Core.Models.Masters
{
    public class BankViewModelCount 
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<BankViewModel> data { get; set; }
    }
}