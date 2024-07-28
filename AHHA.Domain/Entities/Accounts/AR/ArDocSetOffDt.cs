using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArDocSetOffDt
    {
        public byte CompanyId { get; set; }
        public long SetoffId { get; set; }
        public string SetoffNo { get; set; }
        public string ReferenceNo { get; set; }
        public byte ItemNo { get; set; }
        public byte TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public byte DocCurrencyId { get; set; }
        public decimal DocExhRate { get; set; }
        public DateTime DocAccountDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public decimal DocTotAmt { get; set; }
        public decimal DocTotLocalAmt { get; set; }
        public decimal DocBalAmt { get; set; }
        public decimal DocBalLocalAmt { get; set; }
        public decimal AllocAmt { get; set; }
        public decimal AllocLocalAmt { get; set; }
        public decimal DocAllocAmt { get; set; }
        public decimal DocAllocLocalAmt { get; set; }
        public decimal CentDiff { get; set; }
        public decimal ExhGainLoss { get; set; }
    }
}
