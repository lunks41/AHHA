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
        public Int32 GLId { get; set; }
        public Int16 CompanyId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public Int16 AccTypeId { get; set; }
        public Int16 AccGroupId { get; set; }
        public Int16 COACategoryId1 { get; set; }
        public Int16 COACategoryId2 { get; set; }
        public Int16 COACategoryId3 { get; set; }
        public bool IsSysControl { get; set; }
        public Int32 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
