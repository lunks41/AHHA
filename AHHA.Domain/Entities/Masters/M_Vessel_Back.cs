using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_Vessel_Back
    {
        public int VesselId { get; set; }
        public Int16 CompanyId { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public string CallSign { get; set; }
        public string IMOCode { get; set; }
        public string GRT { get; set; }
        public string LicenseNo { get; set; }
        public string VesselType { get; set; }
        public string Flag { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateBy { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}