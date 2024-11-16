namespace AHHA.Core.Models.Account.CB
{
    public class CBGenReceiptViewModel
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CBGenReceiptHdViewModel> data { get; set; }
    }
}