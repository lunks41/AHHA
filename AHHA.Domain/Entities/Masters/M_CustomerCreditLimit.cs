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
    public class M_CustomerCreditLimit
    {
        [Key]
        public int CustomerId { get; set; }
        [Key]
        public byte CompanyId { get; set; }
        public DateTime EffectFrom { get; set; }
        public DateTime EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public decimal CreditLimitAmt { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }

    }
}
