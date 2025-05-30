﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Accounts.AP
{
    public class ApPaymentHd
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 PaymentId { get; set; }

        public string PaymentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public Int16 BankId { get; set; }
        public Int16 PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnAllocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnAllocTotLocalAmt { get; set; }

        public Int16 RecCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal RecExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocTotLocalAmt { get; set; }

        public Int32 BankChargeGLId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesLocalAmt { get; set; }

        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal DocExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotLocalAmt { get; set; }
    }
}