﻿namespace AHHA.API.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public byte CompanyId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditBy { get; set; }
        public DateTime? EditDateId { get; set; }
    }
}
