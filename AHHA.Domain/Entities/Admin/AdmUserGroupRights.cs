using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    [Keyless]
    public class AdmUserGroupRights
    {
        [Key]
        public Int32 UserGroupId { get; set; }
        [Key]
        public Int16 ModuleId { get; set; }
        [Key]
        public Int32 TransactionId { get; set; }
        public bool IsRead { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
