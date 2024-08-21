namespace AHHA.Core.Models.Masters
{
    public class SupplierContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<SupplierContactViewModel> data { get; set; }
    }
}