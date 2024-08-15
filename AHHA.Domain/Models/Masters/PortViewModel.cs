using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class PortViewModel
    {
        public Int32 PortId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int32 PortRegionId { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}
