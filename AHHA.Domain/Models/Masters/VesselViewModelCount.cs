namespace AHHA.Core.Models.Masters
{
    public class VesselViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<VesselViewModel> data { get; set; }
    }
}