﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    public class AdmUser
    {
        [Key]
        public Int32 UserId { get; set; }

        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        [NotMapped]
        public Int32? EditById { get; set; }

        [NotMapped]
        public DateTime? EditDate { get; set; }

        public Int32 UserGroupId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}