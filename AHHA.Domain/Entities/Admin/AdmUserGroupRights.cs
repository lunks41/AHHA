﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    [PrimaryKey(nameof(UserGroupId), nameof(ModuleId), nameof(TransactionId))]
    public class AdmUserGroupRights
    {
        public Int16 UserGroupId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }
    }
}