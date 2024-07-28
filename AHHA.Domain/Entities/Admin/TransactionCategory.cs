using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class TransactionCategory
    {
        [Key]
        public short TransCategoryId { get; set; }
        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public short SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}
