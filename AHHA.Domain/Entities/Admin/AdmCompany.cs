﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Entities.Admin
{
    public class AdmCompany
    {
        [Key]
        public Int16 CompanyId { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNo { get; set; }
        public string TaxRegistrationNo { get; set; }
        public Int64 AddressId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}