using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class AdmErrorLog
    {
        [Key]
        public long ErrId { get; set; }
        public byte CompanyId { get; set; }
        public byte ModuleId { get; set; }
        public short TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string TblName { get; set; }
        public byte ModeId { get; set; }
        public string Remarks { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
