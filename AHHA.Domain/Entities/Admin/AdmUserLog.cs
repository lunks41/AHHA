﻿using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Admin
{
    public class AdmUserLog
    {
        [Key]
        public Int16 UserId { get; set; }

        public bool IsLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public string Remarks { get; set; }
    }
}