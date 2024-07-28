using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_OrderType
    {
        public byte CompanyId { get; set; }
        [Key]
        public short OrderTypeId { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderTypeName { get; set; }
        public byte OrderTypeCategoryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
