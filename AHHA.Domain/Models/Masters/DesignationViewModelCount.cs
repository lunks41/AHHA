namespace AHHA.Core.Models.Masters
{
    public class DesignationViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<DesignationViewModel> data { get; set; }
    }
}