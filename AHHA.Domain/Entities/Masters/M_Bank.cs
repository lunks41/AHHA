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
        public short BankId { get; set; }
        public byte CompanyId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public byte CurrencyId { get; set; }
        public string AccountNo { get; set; }
        public string SwiftCode { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public short GLId { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
