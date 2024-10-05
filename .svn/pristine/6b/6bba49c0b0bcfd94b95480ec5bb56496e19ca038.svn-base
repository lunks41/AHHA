using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.AR
{
    public class ARReceiptViewModel
    {
        public Int16 CompanyId { get; set; }
        public string ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public Int16 BankId { get; set; }
        public Int16 PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public Int16 RecCurrencyId { get; set; }
        public decimal RecExhRate { get; set; }
        public decimal RecTotAmt { get; set; }
        public decimal RecTotLocalAmt { get; set; }
        public decimal ExhGainLoss { get; set; }
        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public string CreateBy { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public string CancelBy { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<ARReceiptDtViewModel> data_details { get; set; }
    }
}