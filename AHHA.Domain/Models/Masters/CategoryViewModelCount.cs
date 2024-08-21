namespace AHHA.Core.Models.Masters
{
    public class CategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CategoryViewModel> data { get; set; }
    }
}