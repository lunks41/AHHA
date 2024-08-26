﻿using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Admin
{
    public class Transaction
    {
        [Key]
        public Int32 TransactionId { get; set; }

        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public Int16 ModuleId { get; set; }
        public Int32 TransCategoryId { get; set; }
        public bool IsNumber { get; set; }
        public Int32 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}