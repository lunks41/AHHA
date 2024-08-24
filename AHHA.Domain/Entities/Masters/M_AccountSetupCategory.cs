﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    public class M_AccountSetupCategory
    {
        [Key]
        public Int16 AccSetupCategoryId { get; set; }

        public string AccSetupCategoryCode { get; set; }
        public string AccSetupCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}