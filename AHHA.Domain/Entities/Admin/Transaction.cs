using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class Transaction
    {
        [Key]
        public short TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public byte ModuleId { get; set; }
        public short TransCategoryId { get; set; }
        public bool IsNumber { get; set; }
        public short SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}
