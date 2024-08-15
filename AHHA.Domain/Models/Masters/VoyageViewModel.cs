using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class VoyageViewModel
    {
        public Int16 CompanyId { get; set; }
        public int VoyageId { get; set; }
        public string VoyageNo { get; set; }
        public string ReferenceNo { get; set; }
        public int VesselId { get; set; }
        public Int32 BargeId { get; set; }
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
