﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Models.Admin
{
    public class UserGroupRightsViewModel
    {
        public Int32 UserGroupId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int32 TransactionId { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}