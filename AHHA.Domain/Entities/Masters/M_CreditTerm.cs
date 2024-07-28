using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_CreditTerm
    {
        [Key]
        public byte CreditTermId { get; set; }
        public byte CompanyId { get; set; }
        public string CreditTermCode { get; set; }
        public string CreditTermName { get; set; }
        public short NoDays { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
