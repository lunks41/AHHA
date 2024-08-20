using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_UomDt
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public Int16 UomId { get; set; }

        [Key]
        public Int16 PackUomId { get; set; }

        public decimal UomFactor { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}