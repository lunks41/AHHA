﻿using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.GL
{
    public class GLOpeningBalanceViewModel
    {
        private DateTime? _accountDate;
        public Int16 CompanyId { get; set; }
        public Int32 DocumentId { get; set; }
        public Int32 ItemNo { get; set; }
        public Int32 GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public string DocumentNo { get; set; }

        public string AccountDate
        {
            get { return _accountDate.HasValue ? DateHelperStatic.FormatDate(_accountDate.Value) : ""; }
            set { _accountDate = string.IsNullOrEmpty(value) ? (DateTime?)null : DateHelperStatic.ParseDBDate(value); }
        }

        public Int32 CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Int32 SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public Int32 DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public Int32 EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Int32 ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Int32 PortId { get; set; }
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
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public byte EditVersion { get; set; }
    }
}