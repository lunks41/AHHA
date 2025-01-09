namespace AHHA.Core.Models.Account.AR
{
    public class ARRefundViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ARRefundViewModel> data { get; set; }
    }
}