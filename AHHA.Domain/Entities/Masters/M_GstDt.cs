﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_GstDt
    {
        [Key]
        public Int16 GstId { get; set; }

        [Key]
        public Int16 CompanyId { get; set; }

        public decimal GstPercentahge { get; set; }
        public DateTime ValidFrom { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}