namespace AHHA.Core.Models.Masters
{
    public class CustomerContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CustomerContactViewModel> data { get; set; }
    }
}