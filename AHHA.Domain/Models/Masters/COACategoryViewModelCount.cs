namespace AHHA.Core.Models.Masters
{
    public class COACategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<COACategoryViewModel> data { get; set; }
    }
}