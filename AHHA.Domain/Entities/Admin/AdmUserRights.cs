﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    [PrimaryKey(nameof(CompanyId), nameof(UserId))]
    public class AdmUserRights
    {
        public Int16 CompanyId { get; set; }
        public Int16 UserId { get; set; }
        public Int16 UserGroupId { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }
    }
}