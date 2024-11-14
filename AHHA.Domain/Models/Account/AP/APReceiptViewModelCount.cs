namespace AHHA.Core.Models.Account.AP
{
    public class APReceiptViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<APReceiptViewModel> data { get; set; }
    }
}