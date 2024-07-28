﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArCreditNoteDt
    {
        public long CreditNoteId { get; set; }
        public string CreditNoteNo { get; set; }
        public short ItemNo { get; set; }
        public short SeqNo { get; set; }
        public short DocItemNo { get; set; }
        public int ProductId { get; set; }
        public short GLId { get; set; }
        public decimal QTY { get; set; }
        public decimal BillQTY { get; set; }
        public byte UomId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCurAmt { get; set; }
        public string Remarks { get; set; }
        public byte GstId { get; set; }
        public decimal GstPercentage { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCurAmt { get; set; }
        public DateTime DeliveryDate { get; set; }
        public short DepartmentId { get; set; }
        public short EmployeeId { get; set; }
        public short PortId { get; set; }
        public int VesselId { get; set; }
        public short BargeId { get; set; }
        public int VoyageId { get; set; }
        public long OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public long SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string SupplierName { get; set; }
        public string SuppCreditNoteNo { get; set; }
        public long APCreditNoteId { get; set; }
        public string APCreditNoteNo { get; set; }
    }
}
