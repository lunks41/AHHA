using AHHA.Core.Common;

namespace AHHA.Core.Models.Masters
{
    public class PortRegionViewModelCount 
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<PortRegionViewModel> data { get; set; }
    }
}