using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLTransactions
    {
        public Int16 CompanyId { get; set; }
        public long DocumentId { get; set; }
        public Int32 ItemNo { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public string DocumentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public int CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal CtyExhRate { get; set; }
        public Int32 BankId { get; set; }
        public Int32 GLId { get; set; }
        public bool IsDebit { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCtyAmt { get; set; }
        public Int16 GstId { get; set; }
        public DateTime GstClaimDate { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCtyAmt { get; set; }
        public string Remarks { get; set; }
        public Int32 DepartmentId { get; set; }
        public Int32 EmployeeId { get; set; }
        public Int32 PortId { get; set; }
        public int VesselId { get; set; }
        public Int32 BargeId { get; set; }
        public string PaymentFromTo { get; set; }
        public string PaymentType { get; set; }
        public string PaymentNo { get; set; }
        public bool IsSystem { get; set; }
        public bool IsMaster { get; set; }
        public string ModuleFrom { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
