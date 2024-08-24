﻿using Microsoft.EntityFrameworkCore;

namespace AHHA.Core.Entities.Admin
{
    [Keyless]
    public class ShareData
    {
        public Int16 ModuleId { get; set; }
        public Int32 TransactionId { get; set; }
        public bool ShareToAll { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 SetId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}