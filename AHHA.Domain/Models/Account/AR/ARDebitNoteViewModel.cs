﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.AR
{
    public class ARDebitNoteViewModel
    {
        public Int16 CompanyId { get; set; }
        public string DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int32 CustomerId { get; set; }
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

        public DateTime GstClaimDate { get; set; }

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

        public string SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
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
        public string SupplierName { get; set; }
        public string SuppDebitNoteNo { get; set; }
        public string APDebitNoteId { get; set; }
        public string APDebitNoteNo { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16 CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<ARDebitNoteDtViewModel> data_details { get; set; }
    }
}