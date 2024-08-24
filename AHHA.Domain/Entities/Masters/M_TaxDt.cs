﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_TaxDt
    {
        [Key]
        public Int16 TaxId { get; set; }

        [Key]
        public Int16 CompanyId { get; set; }

        public decimal TaxPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public Int32 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}