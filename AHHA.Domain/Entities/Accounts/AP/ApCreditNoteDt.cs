﻿using Microsoft.EntityFrameworkCore;

namespace AHHA.Core.Entities.Accounts.AP
{
    [PrimaryKey(nameof(CreditNoteId), nameof(ItemNo))]
    public class ApCreditNoteDt
    {
        public Int64 CreditNoteId { get; set; }
        public string CreditNoteNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }
        public Int32 DocItemNo { get; set; }
        public Int16 ProductId { get; set; }
        public Int16 GLId { get; set; }
        public decimal QTY { get; set; }
        public decimal BillQTY { get; set; }
        public Int16 UomId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCtyAmt { get; set; }
        public string Remarks { get; set; }
        public Int16 GstId { get; set; }
        public decimal GstPercentage { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCtyAmt { get; set; }
        public DateTime DeliveryDate { get; set; }
        public Int16 DepartmentId { get; set; }
        public Int16 EmployeeId { get; set; }
        public Int16 PortId { get; set; }
        public Int32 VesselId { get; set; }
        public Int16 BargeId { get; set; }
        public Int16 VoyageId { get; set; }
        public Int64 OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public Int64 PurOrderId { get; set; }
        public string PurOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string CustomerName { get; set; }
        public string CustInvoiceNo { get; set; }
        public Int64 ArInvoiceId { get; set; }
        public string ArInvoiceNo { get; set; }
        public byte EditVersion { get; set; }
    }
}