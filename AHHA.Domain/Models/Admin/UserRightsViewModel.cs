﻿namespace AHHA.Core.Models.Admin
{
    public class UserRightsViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16 UserGroupId { get; set; }
        public string CreateBy { get; set; }
    }
}