using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class AdmTransaction
    {
        [Key]
        public Int32 TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public Int16 ModuleId { get; set; }
        public Int32 TransCategoryId { get; set; }
        public bool IsNumber { get; set; }
        public Int32 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public Int32 CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}
