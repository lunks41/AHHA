﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    [PrimaryKey(nameof(ModuleId), nameof(TransactionId), nameof(CompanyId), nameof(SetId))]
    public class AdmShareData
    {
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public bool ShareToAll { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 SetId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}