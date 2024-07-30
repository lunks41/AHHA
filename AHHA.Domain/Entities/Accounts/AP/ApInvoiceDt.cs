using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AP
{
    public class ApInvoiceDt
    {
        public long InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int32 SeqNo { get; set; }
        public Int32 DocItemNo { get; set; }
        public int ProductId { get; set; }
        public Int32 GLId { get; set; }
        public decimal QTY { get; set; }
        public decimal BillQTY { get; set; }
        public Int16 UomId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCurAmt { get; set; }
        public string Remarks { get; set; }
        public Int16 GstId { get; set; }
        public decimal GstPercentage { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCurAmt { get; set; }
        public DateTime DeliveryDate { get; set; }
        public Int32 DepartmentId { get; set; }
        public Int32 EmployeeId { get; set; }
        public Int32 PortId { get; set; }
        public int VesselId { get; set; }
        public Int32 BargeId { get; set; }
        public int VoyageId { get; set; }
        public long OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public long PurOrderId { get; set; }
        public string PurOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string CustomerName { get; set; }
        public string CustInvoiceNo { get; set; }
        public long ArInvoiceId { get; set; }
        public string ArInvoiceNo { get; set; }
    }
}
