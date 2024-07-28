using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.CB
{
    public class CBBankTransfer
    {
        public byte CompanyId { get; set; }
        public long TransferId { get; set; }
        public string TransferNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public short FromBankId { get; set; }
        public byte FromCurrencyId { get; set; }
        public decimal FromExhRate { get; set; }
        public short PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public decimal FromBankChgAmt { get; set; }
        public decimal FromBankChgLocalAmt { get; set; }
        public decimal FromTotAmt { get; set; }
        public decimal FromTotLocalAmt { get; set; }
        public short ToBankId { get; set; }
        public byte ToCurrencyId { get; set; }
        public decimal ToExhRate { get; set; }
        public decimal ToBankChgAmt { get; set; }
        public decimal ToBankChgLocalAmt { get; set; }
        public decimal ToTotAmt { get; set; }
        public decimal ToTotLocalAmt { get; set; }
        public string Remarks { get; set; }
        public string PayeeTo { get; set; }
        public decimal ExhGainLoss { get; set; }
        public string ModuleFrom { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short CancelById { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
    }
}
