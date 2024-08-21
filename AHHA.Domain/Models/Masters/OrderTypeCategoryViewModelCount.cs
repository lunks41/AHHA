namespace AHHA.Core.Models.Masters
{
    public class OrderTypeCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<OrderTypeCategoryViewModel> data { get; set; }
    }
}