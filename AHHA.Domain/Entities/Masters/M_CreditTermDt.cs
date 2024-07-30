﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    [Keyless]
    public class M_CreditTermDt
    {
        public Int16 CreditTermId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 FromDay { get; set; }
        public Int16 ToDay { get; set; }
        public bool IsEndOfMonth { get; set; }
        public Int16 DueDay { get; set; }
        public Int16 NoMonth { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}