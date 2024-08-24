using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    public class AdmTransactionCategory
    {
        [Key]
        public Int32 TransCategoryId { get; set; }

        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public Int32 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        [NotMapped]
        public DateTime CreateDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}