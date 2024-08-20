namespace AHHA.Core.Entities.Accounts.CB
{
    public class CBGenPaymentDt
    {
        public long PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int32 SeqNo { get; set; }
        public Int32 GLId { get; set; }
        public string Remarks { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal TotCurAmt { get; set; }
        public Int16 GstId { get; set; }
        public decimal GstPercentage { get; set; }
        public decimal GstAmt { get; set; }
        public decimal GstLocalAmt { get; set; }
        public decimal GstCurAmt { get; set; }
        public Int32 BargeId { get; set; }
        public Int32 DepartmentId { get; set; }
        public Int32 EmployeeId { get; set; }
        public int VesselId { get; set; }
        public int VoyageId { get; set; }
    }
}