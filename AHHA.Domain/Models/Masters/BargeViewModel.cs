﻿namespace AHHA.Core.Models.Masters
{
    public class BargeViewModel
    {
        public Int16 BargeId { get; set; }
        public Int16 CompanyId { get; set; }
        public string BargeCode { get; set; }
        public string BargeName { get; set; }
        public string CallSign { get; set; }
        public string IMOCode { get; set; }
        public string GRT { get; set; }
        public string LicenseNo { get; set; }
        public string BargeType { get; set; }
        public string Flag { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwn { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}