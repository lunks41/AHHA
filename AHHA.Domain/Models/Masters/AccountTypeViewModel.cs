﻿namespace AHHA.Core.Models.Masters
{
    public class AccountTypeViewModel
    {
        public Int16 AccTypeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string AccTypeCode { get; set; }
        public string AccTypeName { get; set; }
        public Int16 SeqNo { get; set; }
        public string AccGroupName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}