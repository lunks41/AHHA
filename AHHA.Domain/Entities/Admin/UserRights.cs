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
    public class UserRights
    {
        [Key]
        public Int16 CompanyId { get; set; }
        [Key]
        public Int32 UserId { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 UserGroupId { get; set; }
    }
}
