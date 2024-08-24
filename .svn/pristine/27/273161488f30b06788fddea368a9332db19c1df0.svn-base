using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    public class AdmAccountType
    {
        [Key]
        public Int16 AccTypeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string AccTypeCode { get; set; }
        public string AccTypeName { get; set; }
        public Int16 SeqNo { get; set; }
        public string AccGroupName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        [NotMapped]
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}