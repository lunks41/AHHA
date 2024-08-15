using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class BankViewModel
    {
        public Int32 BankId { get; set; }
        public Int16 CompanyId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string AccountNo { get; set; }
        public string SwiftCode { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public Int32 GLId { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}
