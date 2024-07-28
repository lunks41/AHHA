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
        public short PortRegionId { get; set; }
        public byte CompanyId { get; set; }
        public string PortRegionCode { get; set; }
        public string PortRegionName { get; set; }
        public short CountryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
