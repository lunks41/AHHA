﻿namespace AHHA.Core.Models.Setting
{
    public class UserGridViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public string GrdName { get; set; }
        public string GrdString { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}