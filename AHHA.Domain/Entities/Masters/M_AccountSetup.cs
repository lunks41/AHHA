﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_AccountSetup
    {
        [Key]
        public Int16 AccSetupId { get; set; }
        public Int16 CompanyId { get; set; }
        public string AccSetupCode { get; set; }
        public string AccSetupName { get; set; }
        public Int16 AccSetupCategoryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
