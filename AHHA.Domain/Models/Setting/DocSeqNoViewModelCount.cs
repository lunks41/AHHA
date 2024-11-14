namespace AHHA.Core.Models.Setting
{
    public class DocSeqNoViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int32 totalRecords { get; set; }
        public List<DynamicLookupViewModel> data { get; set; }
    }
}