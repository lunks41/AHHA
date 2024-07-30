using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_Bank
    {
        [Key]
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
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
