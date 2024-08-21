namespace AHHA.Core.Models.Masters
{
    public class TaxViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<TaxViewModel> data { get; set; }
    }
}