using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLTransactions
    {
        public byte CompanyId { get; set; }
        public long DocumentId { get; set; }
        public short ItemNo { get; set; }
        public byte ModuleId { get; set; }
        public byte TransactionId { get; set; }
        public string DocumentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public int CustomerId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal CtyExhRate { get; set; }
        public short BankId { get; set; }
        public short GLId { get; set; }
        public bool IsDebit { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCtyAmt { get; set; }
        public byte GstId { get; set; }
        public DateTime GstClaimDate { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCtyAmt { get; set; }
        public string Remarks { get; set; }
        public short DepartmentId { get; set; }
        public short EmployeeId { get; set; }
        public short PortId { get; set; }
        public int VesselId { get; set; }
        public short BargeId { get; set; }
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
