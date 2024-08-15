﻿namespace AHHA.Core.Models.Masters
{
    public class GstCategoryViewModel
    {
        public Int16 GstCategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string GstCategoryCode { get; set; }
        public string GstCategoryName { get; set; }
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