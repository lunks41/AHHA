using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_GroupCreditLimt_Customer
    {
        [Key]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int32 GroupCreditLimitId { get; set; }

        [Key]
        public int CustomerId { get; set; }

        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}