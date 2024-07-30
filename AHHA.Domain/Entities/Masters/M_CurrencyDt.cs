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
    public class M_CurrencyDt
    {
        [Key]
        public Int16 CurrencyId { get; set; }
        [Key]
        public Int16 CompanyId { get; set; }
        public decimal ExhRate { get; set; }
        public DateTime ValidFrom { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
