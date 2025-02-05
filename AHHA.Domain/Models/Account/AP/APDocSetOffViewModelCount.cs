namespace AHHA.Core.Models.Account.AP
{
    public class APDocSetOffViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<APDocSetOffViewModel> data { get; set; }
    }
}