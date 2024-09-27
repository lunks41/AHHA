﻿namespace AHHA.Core.Models.Admin
{
    public class UserRightsViewModel
    {
        public Int16 CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public bool Access { get; set; }
        public Int16 UserId { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
    }
}