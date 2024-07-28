using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_AccountSetup
    {
        [Key]
        public byte AccSetupId { get; set; }
        public byte CompanyId { get; set; }
        public string AccSetupCode { get; set; }
        public string AccSetupName { get; set; }
        public byte AccSetupCategoryId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
