namespace AHHA.Core.Models.Masters
{
    public class CreditTermViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CreditTermViewModel> data { get; set; }
    }
}