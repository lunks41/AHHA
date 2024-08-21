namespace AHHA.Core.Models.Masters
{
    public class UomViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<UomViewModel> data { get; set; }
    }
}