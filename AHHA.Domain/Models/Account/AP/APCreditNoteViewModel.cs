﻿using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.AP
{
    public class APCreditNoteViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _deliveryDate;
        private DateTime _dueDate;
        private DateTime _gstClaimDate;
        public Int16 CompanyId { get; set; }
        public string CreditNoteId { get; set; }
        public string CreditNoteNo { get; set; }
        public string ReferenceNo { get; set; }
        public string SuppInvoiceNo { get; set; }

        public string TrnDate
        {
            get { return DateHelperStatic.FormatDate(_trnDate); }
            set { _trnDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string DeliveryDate
        {
            get { return DateHelperStatic.FormatDate(_deliveryDate); }
            set { _deliveryDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string DueDate
        {
            get { return DateHelperStatic.FormatDate(_dueDate); }
            set { _dueDate = DateHelperStatic.ParseDBDate(value); }
        }

        public Int32 SupplierId { get; set; }
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        public Int16 CreditTermId { get; set; }
        public Int16 BankId { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceNo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string GstClaimDate
        {
            get { return DateHelperStatic.FormatDate(_gstClaimDate); }
            set { _gstClaimDate = DateHelperStatic.ParseDBDate(value); }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmtAftGst { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmtAftGst { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmtAftGst { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BalLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PayAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PayLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExGainLoss { get; set; }

        public string PurchaseOrderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string OperationId { get; set; }
        public string OperationNo { get; set; }
        public string Remarks { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string ContactName { get; set; }
        public string MobileNo { get; set; }
        public string EmailAdd { get; set; }
        public string ModuleFrom { get; set; }
        public string CustomerName { get; set; }
        public string ArInvoiceId { get; set; }
        public string ArInvoiceNo { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16 CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<APCreditNoteDtViewModel> data_details { get; set; }
    }
}