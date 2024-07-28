using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.CB
{
    public class CBPettyCashHd
    {
        public byte CompanyId { get; set; }
        public long ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public short BankId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal CtyExhRate { get; set; }
        public short PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public decimal BankChgAmt { get; set; }
        public decimal BankChgLocalAmt { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCtyAmt { get; set; }
        public DateTime GstClaimDate { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCtyAmt { get; set; }
        public decimal TotAmtAftGst { get; set; }
        public decimal TotLocalAmtAftGst { get; set; }
        public decimal TotCtyAmtAftGst { get; set; }
        public string Remarks { get; set; }
        public string PayeeTo { get; set; }
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
