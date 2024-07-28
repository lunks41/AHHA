﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArReceiptHd
    {
        public byte CompanyId { get; set; }
        public long ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public short BankId { get; set; }
        public short PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public int CustomerId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public byte RecCurrencyId { get; set; }
        public decimal RecExhRate { get; set; }
        public decimal RecTotAmt { get; set; }
        public decimal RecTotLocalAmt { get; set; }
        public decimal ExhGainLoss { get; set; }
        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
        public bool IsCancel { get; set; }
        public string CancelBy { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
    }
}
