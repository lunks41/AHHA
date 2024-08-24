﻿namespace AHHA.Core.Models
{
    public class CountryViewModel
    {
        public Int32 CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public Int16 CompanyId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}