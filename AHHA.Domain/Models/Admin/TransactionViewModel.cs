﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Admin
{
    public class TransactionViewModel
    {
        public Int32 TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public Int16 ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public Int32 TransCategoryId { get; set; }
        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public Int32 SeqNo { get; set; }
        public Int32 TransCatSeqNo { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
    }
}