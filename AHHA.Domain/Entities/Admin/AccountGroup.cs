using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class AccountGroup
    {
        [Key]
        public byte AccGroupId { get; set; }
        public byte CompanyId { get; set; }
        public string AccGroupCode { get; set; }
        public string AccGroupName { get; set; }
        public byte SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
