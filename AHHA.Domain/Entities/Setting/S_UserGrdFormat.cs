﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_UserGrdFormat
    {
        public Int16 CompanyId { get; set; }
        public Int32 UserId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int32 TransactionId { get; set; }
        public string GrdName { get; set; }
        public string GrdString { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}