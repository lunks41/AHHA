using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    public class M_Barge
    {
        [Key]
        public Int32 BargeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string BargeCode { get; set; }
        public string BargeName { get; set; }
        public string CallSign { get; set; }
        public string IMOCode { get; set; }
        public string GRT { get; set; }
        public string LicenseNo { get; set; }
        public string BargeIType { get; set; }
        public string Flag { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public Int32 CreateById { get; set; }

        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}