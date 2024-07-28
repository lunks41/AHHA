using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public byte CompanyId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOtherName { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerRegNo { get; set; }
        public byte CurrencyId { get; set; }
        public byte CreditTermId { get; set; }
        public int ParentCustomerId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public bool IsSupplier { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
