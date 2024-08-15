﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class ChartOfAccountViewModel
    {
        public Int32 GLId { get; set; }
        public Int16 CompanyId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public Int16 AccTypeId { get; set; }
        public Int16 AccGroupId { get; set; }
        public Int16 COACategoryId1 { get; set; }
        public Int16 COACategoryId2 { get; set; }
        public Int16 COACategoryId3 { get; set; }
        public bool IsSysControl { get; set; }
        public Int32 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}