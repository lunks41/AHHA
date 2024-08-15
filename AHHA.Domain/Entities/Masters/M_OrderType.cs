using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_OrderType
    {
        public Int16 CompanyId { get; set; }
        [Key]
        public Int32 OrderTypeId { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderTypeName { get; set; }
        public Int16 OrderTypeCategoryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        [NotMapped]
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
