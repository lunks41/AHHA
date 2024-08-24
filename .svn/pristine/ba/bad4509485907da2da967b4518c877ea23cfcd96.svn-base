using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    public class M_TaxCategory
    {
        [Key]
        public Int16 TaxCategoryId { get; set; }

        public Int16 CompanyId { get; set; }
        public string TaxCategoryCode { get; set; }
        public string TaxCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}