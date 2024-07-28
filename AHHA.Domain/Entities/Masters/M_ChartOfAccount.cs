using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_ChartOfAccount
    {
        [Key]
        public short GLId { get; set; }
        public byte CompanyId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public byte AccTypeId { get; set; }
        public byte AccGroupId { get; set; }
        public byte COACategoryId1 { get; set; }
        public byte COACategoryId2 { get; set; }
        public byte COACategoryId3 { get; set; }
        public bool IsSysControl { get; set; }
        public short SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
