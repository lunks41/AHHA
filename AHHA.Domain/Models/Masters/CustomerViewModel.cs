﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public Int16 CompanyId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOtherName { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerRegNo { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 CreditTermId { get; set; }
        public int ParentCustomerId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public bool IsSupplier { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}