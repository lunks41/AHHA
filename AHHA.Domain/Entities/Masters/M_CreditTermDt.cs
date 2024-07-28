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
    public class M_CreditTermDt
    {
        public byte CreditTermId { get; set; }
        public byte CompanyId { get; set; }
        public byte FromDay { get; set; }
        public byte ToDay { get; set; }
        public bool IsEndOfMonth { get; set; }
        public byte DueDay { get; set; }
        public byte NoMonth { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
