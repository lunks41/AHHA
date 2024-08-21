namespace AHHA.Core.Models.Masters
{
    public class PortViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<PortViewModel> data { get; set; }
    }
}