using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.GL
{
    public class GLOpeningBalanceViewModel
    {
        public short CompanyId { get; set; }
        public short CurrencyId { get; set; }
        public int FinYear { get; set; }
        public short FinMonth { get; set; }
        public short GLId { get; set; }
        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }
    }
}