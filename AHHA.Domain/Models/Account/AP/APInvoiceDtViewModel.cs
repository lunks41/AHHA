﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.AP
{
    public class APInvoiceDtViewModel
    {
        public string InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }
        public Int16 DocItemNo { get; set; }
        public Int16 ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Int16 GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal QTY { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal BillQTY { get; set; }

        public Int16 UomId { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string Remarks { get; set; }
        public byte GstId { get; set; }
        public string GstCode { get; set; }
        public string GstName { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        public DateTime DeliveryDate { get; set; }
        public Int16 DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public Int16 EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Int16 PortId { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public Int32 VesselId { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public Int16 BargeId { get; set; }
        public string BargeCode { get; set; }
        public string BargeName { get; set; }
        public Int32 VoyageId { get; set; }
        public string VoyageNo { get; set; }
        public string VoyageReferenceNo { get; set; }
        public Int64 OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public Int64 SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string SupplierName { get; set; }
        public string SuppInvoiceNo { get; set; }
        public Int64 APInvoiceId { get; set; }
        public string APInvoiceNo { get; set; }
        public byte EditVersion { get; set; }
    }
}