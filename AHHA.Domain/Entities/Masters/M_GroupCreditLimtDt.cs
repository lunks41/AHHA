using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_GroupCreditLimtDt
    {
        [Key]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int32 GroupCreditLimitId { get; set; }

        public DateTime EffectFrom { get; set; }
        public DateTime EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public decimal CreditLimitAmt { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}