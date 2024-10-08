﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    [PrimaryKey(nameof(ModuleId), nameof(TransactionId))]
    public class AdmTransaction
    {
        public Int16 TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public Int16 ModuleId { get; set; }

        [ForeignKey("CategoryId")]
        public Int32 TransCategoryId { get; set; }

        public bool IsNumber { get; set; }
        public Int16 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        public Int32 CreateBy { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}