using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_SupplierAddress
    {
        [Key]
        public int SupplierId { get; set; }
        [Key]
        public short AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PinCode { get; set; }
        public short CountryId { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailAdd { get; set; }
        public string WebUrl { get; set; }
        public bool IsDefaultAdd { get; set; }
        public bool IsDeliveryAdd { get; set; }
        public bool IsFinAdd { get; set; }
        public bool IsSalesAdd { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
