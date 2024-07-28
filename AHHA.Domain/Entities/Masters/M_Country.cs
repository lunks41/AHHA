using AHHA.Core.Entities.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_Country
    {
        [Key]
        public short CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public byte CompanyId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
