﻿namespace AHHA.Core.Models.Masters
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public short CompanyId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public int CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditBy { get; set; }
        public DateTime? EditDateId { get; set; }
    }
}