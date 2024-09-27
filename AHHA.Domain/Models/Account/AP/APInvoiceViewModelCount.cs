namespace AHHA.Core.Models.Account.AP
{
    public class APInvoiceViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<APInvoiceViewModel> data { get; set; }
    }
}