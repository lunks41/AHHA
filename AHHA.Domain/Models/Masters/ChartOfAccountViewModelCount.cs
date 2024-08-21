namespace AHHA.Core.Models.Masters
{
    public class ChartOfAccountViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ChartOfAccountViewModel> data { get; set; }
    }
}