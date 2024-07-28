using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_FinSettings
    {
        public byte CompanyId { get; set; }
        public byte Base_CurrencyId { get; set; }
        public byte Local_CurrencyId { get; set; }
        public short ExhGainLoss_GlId { get; set; }
        public short BankCharge_GlId { get; set; }
        public short ProfitLoss_GlId { get; set; }
        public short RetEarning_GlId { get; set; }
        public short SaleGst_GlId { get; set; }
        public short PurGst_GlId { get; set; }
        public short SaleDef_GlId { get; set; }
        public short PurDef_GlId { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
