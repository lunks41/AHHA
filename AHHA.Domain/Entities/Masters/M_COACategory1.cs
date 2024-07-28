﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_COACategory1
    {
        [Key]
        public byte COACategoryId { get; set; }
        public byte CompanyId { get; set; }
        public string COACategoryCode { get; set; }
        public string COACategoryName { get; set; }
        public short SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
