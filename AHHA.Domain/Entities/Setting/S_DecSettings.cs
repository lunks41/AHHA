using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_DecSettings
    {
        public byte CompanyId { get; set; }
        public byte AmtDec { get; set; }
        public string LocAmtDec { get; set; }
        public byte PriceDec { get; set; }
        public byte QtyDec { get; set; }
        public byte ExhRateDec { get; set; }
        public string DateFormat { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
