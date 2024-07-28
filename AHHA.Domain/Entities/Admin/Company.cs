using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class Company
    {
        [Key]
        public byte CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNo { get; set; }
        public string TaxRegistrationNo { get; set; }
        public long AddressId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
