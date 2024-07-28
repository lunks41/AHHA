using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLBalances_GLCode
    {
        public byte CompanyId { get; set; }
        public byte CurrencyId { get; set; }
        public short FinYear { get; set; }
        public byte FinMonth { get; set; }
        public short GLId { get; set; }
        public bool IsDebit { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
    }
}
