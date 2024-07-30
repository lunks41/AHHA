﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AP
{
    public class ApRefundHd
    {
        public Int16 CompanyId { get; set; }
        public long RefundId { get; set; }
        public string RefundNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public Int32 BankId { get; set; }
        public Int32 PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public int SupplierId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public Int16 PayCurrencyId { get; set; }
        public decimal PayExhRate { get; set; }
        public decimal PayTotAmt { get; set; }
        public decimal PayTotLocalAmt { get; set; }
        public decimal ExhGainLoss { get; set; }
        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int32 CancelById { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
    }
}