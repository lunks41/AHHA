﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Masters
{
    [PrimaryKey(nameof(CurrencyId), nameof(CompanyId), nameof(ValidFrom))]
    public class M_CurrencyLocalDt
    {
        public Int16 CurrencyId { get; set; }
        public Int16 CompanyId { get; set; }
        public decimal ExhRate { get; set; }
        public DateTime ValidFrom { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}