using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_DecSettings
    {
        public Int16 CompanyId { get; set; }
        public Int16 AmtDec { get; set; }
        public string LocAmtDec { get; set; }
        public Int16 PriceDec { get; set; }
        public Int16 QtyDec { get; set; }
        public Int16 ExhRateDec { get; set; }
        public string DateFormat { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
