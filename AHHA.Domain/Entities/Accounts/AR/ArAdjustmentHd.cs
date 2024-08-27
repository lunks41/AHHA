﻿namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArAdjustmentHd
    {
        public Int16 CompanyId { get; set; }
        public long AdjustmentId { get; set; }
        public string AdjustmentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal CtyExhRate { get; set; }
        public Int16 CreditTermId { get; set; }
        public Int32 BankId { get; set; }
        public bool IsDebit { get; set; }
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
        public decimal BalAmt { get; set; }
        public decimal BalLocalAmt { get; set; }
        public decimal PayAmt { get; set; }
        public decimal PayLocalAmt { get; set; }
        public decimal ExGainLoss { get; set; }
        public long SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public long OperationId { get; set; }
        public string OperationNo { get; set; }
        public string Remarks { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PinCode { get; set; }
        public Int32 CountryId { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string ContactName { get; set; }
        public string MobileNo { get; set; }
        public string EmailAdd { get; set; }
        public string ModuleFrom { get; set; }
        public string SupplierName { get; set; }
        public string SuppAdjustmentNo { get; set; }
        public long APAdjustmentId { get; set; }
        public string APAdjustmentNo { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int32 CancelById { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
    }
}