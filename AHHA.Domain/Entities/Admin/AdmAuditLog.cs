using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class AdmAuditLog
    {
        [Key]
        public long AuditId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int32 TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string TblName { get; set; }
        public Int16 ModeId { get; set; }
        public string Remarks { get; set; }
        public Int32 CreateById { get; set; }
        [NotMapped]
        public DateTime CreateDate { get; set; }
    }
}
