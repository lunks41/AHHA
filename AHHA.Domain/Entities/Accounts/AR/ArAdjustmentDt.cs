﻿namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArAdjustmentDt
    {
        public Int64 AdjustmentId { get; set; }
        public string AdjustmentNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int32 SeqNo { get; set; }
        public Int32 DocItemNo { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 GLId { get; set; }
        public decimal QTY { get; set; }
        public decimal BillQTY { get; set; }
        public Int16 UomId { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsDebit { get; set; }
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
        public Int32 DepartmentId { get; set; }
        public Int32 EmployeeId { get; set; }
        public Int32 PortId { get; set; }
        public Int32 VesselId { get; set; }
        public Int32 BargeId { get; set; }
        public Int32 VoyageId { get; set; }
        public Int64 OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public Int64 SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string SupplierName { get; set; }
        public string SuppAdjustmentNo { get; set; }
    }
}