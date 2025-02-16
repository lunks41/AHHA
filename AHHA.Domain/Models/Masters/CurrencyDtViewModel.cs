﻿using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Masters
{
    public class CurrencyDtViewModel
    {
        private DateTime _validFrom;
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public Int16 CompanyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        public string ValidFrom
        {
            get { return DateHelperStatic.FormatDate(_validFrom); }
            set { _validFrom = DateHelperStatic.ParseDBDate(value); }
        }

        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}