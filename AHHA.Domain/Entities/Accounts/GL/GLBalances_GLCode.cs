using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLBalances_GLCode
    {
        public Int16 CompanyId { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int32 FinYear { get; set; }
        public Int16 FinMonth { get; set; }
        public Int32 GLId { get; set; }
        public bool IsDebit { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
    }
}
