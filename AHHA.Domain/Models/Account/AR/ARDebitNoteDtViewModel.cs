using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.AR
{
    public class ARDebitNoteDtViewModel
    {
        public string DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }
        public Int32 DocItemNo { get; set; }
        public Int16 ProductId { get; set; }
        public Int16 GLId { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal QTY { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal BillQTY { get; set; }

        [ForeignKey(nameof(UomId))]
        public Int16 UomId { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string Remarks { get; set; }

        [ForeignKey(nameof(UomId))]
        public Int16 GstId { get; set; }

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
        public Int16 EmployeeId { get; set; }
        public Int16 PortId { get; set; }
        public Int32 VesselId { get; set; }
        public Int16 BargeId { get; set; }
        public Int16 VoyageId { get; set; }
        public string OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public string SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string SupplierName { get; set; }
        public string SuppDebitNoteNo { get; set; }
        public string APDebitNoteId { get; set; }
        public string APDebitNoteNo { get; set; }
        public byte EditVersion { get; set; }
    }
}