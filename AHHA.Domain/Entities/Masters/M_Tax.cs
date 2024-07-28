using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_Tax
    {
        [Key]
        public byte TaxId { get; set; }
        public byte CompanyId { get; set; }
        public byte TaxCategoryId { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
