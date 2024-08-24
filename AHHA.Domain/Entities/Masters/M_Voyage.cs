using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    public class M_Voyage
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public int VoyageId { get; set; }

        public string VoyageNo { get; set; }
        public string ReferenceNo { get; set; }
        public int VesselId { get; set; }
        public Int32 BargeId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}