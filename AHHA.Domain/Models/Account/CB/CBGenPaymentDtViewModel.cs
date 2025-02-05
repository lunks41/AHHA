﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.CB
{
    public class CBGenPaymentDtViewModel
    {
        public string PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }
        public Int16 GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public string Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public byte GstId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        public Int16 BargeId { get; set; }
        public Int16 DepartmentId { get; set; }
        public Int16 EmployeeId { get; set; }
        public Int16 PortId { get; set; }
        public int VesselId { get; set; }
        public Int16 VoyageId { get; set; }
        public byte EditVersion { get; set; }
    }
}