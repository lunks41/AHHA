using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_TaxDt
    {
        [Key]
        public byte TaxId { get; set; }
        [Key]
        public byte CompanyId { get; set; }
        public decimal TaxPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
