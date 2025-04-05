using Microsoft.EntityFrameworkCore;

namespace AHHA.Core.Entities.Accounts.AR
{
    [PrimaryKey(nameof(SetoffId), nameof(ItemNo))]
    public class ArDocSetOffDt
    {
        public Int16 CompanyId { get; set; }
        public Int64 SetoffId { get; set; }
        public string SetoffNo { get; set; }
        public string ReferenceNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public Int16 DocCurrencyId { get; set; }
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
        public byte EditVersion { get; set; }
    }
}