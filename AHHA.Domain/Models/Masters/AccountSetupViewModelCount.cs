using AHHA.Core.Common;

namespace AHHA.Core.Models.Masters
{
    public class AccountSetupViewModelCount 
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<AccountSetupViewModel> data { get; set; }
    }
}