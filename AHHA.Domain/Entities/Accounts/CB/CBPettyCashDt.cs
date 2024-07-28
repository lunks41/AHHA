using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.CB
{
    public class CBPettyCashDt
    {
        public long PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public byte ItemNo { get; set; }
        public short SeqNo { get; set; }
        public short GLId { get; set; }
        public string Remarks { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCurAmt { get; set; }
        public byte GstId { get; set; }
        public decimal GstPercentage { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCurAmt { get; set; }
        public short BargeId { get; set; }
        public short DepartmentId { get; set; }
        public short EmployeeId { get; set; }
        public int VesselId { get; set; }
        public int VoyageId { get; set; }
    }
}
