using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_PortRegion
    {
        [Key]
        public Int32 PortRegionId { get; set; }
        public Int16 CompanyId { get; set; }
        public string PortRegionCode { get; set; }
        public string PortRegionName { get; set; }
        public Int32 CountryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
