using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_NumberFormat
    {
        public short NumberId { get; set; }
        public byte CompanyId { get; set; }
        public byte ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string Prefix { get; set; }
        public byte PrefixSeq { get; set; }
        public string PrefixDelimiter { get; set; }
        public bool IncludeYear { get; set; }
        public byte YearSeq { get; set; }
        public string YearFormat { get; set; }
        public string YearDelimiter { get; set; }
        public bool IncludeMonth { get; set; }
        public byte MonthSeq { get; set; }
        public string MonthFormat { get; set; }
        public string MonthDelimiter { get; set; }
        public byte NoDIgits { get; set; }
        public byte DIgitSeq { get; set; }
        public bool ResetYearly { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
