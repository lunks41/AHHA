﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.CB
{
    public class CBBankTransfer
    {
        public Int16 CompanyId { get; set; }
        public long TransferId { get; set; }
        public string TransferNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public Int32 FromBankId { get; set; }
        public Int16 FromCurrencyId { get; set; }
        public decimal FromExhRate { get; set; }
        public Int32 PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public decimal FromBankChgAmt { get; set; }
        public decimal FromBankChgLocalAmt { get; set; }
        public decimal FromTotAmt { get; set; }
        public decimal FromTotLocalAmt { get; set; }
        public Int32 ToBankId { get; set; }
        public Int16 ToCurrencyId { get; set; }
        public decimal ToExhRate { get; set; }
        public decimal ToBankChgAmt { get; set; }
        public decimal ToBankChgLocalAmt { get; set; }
        public decimal ToTotAmt { get; set; }
        public decimal ToTotLocalAmt { get; set; }
        public string Remarks { get; set; }
        public string PayeeTo { get; set; }
        public decimal ExhGainLoss { get; set; }
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