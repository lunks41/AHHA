using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Masters
{
    public class M_GroupCreditLimt
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public Int32 GroupCreditLimitId { get; set; }

        public string GroupCreditLimitCode { get; set; }
        public string GroupCreditLimitName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}