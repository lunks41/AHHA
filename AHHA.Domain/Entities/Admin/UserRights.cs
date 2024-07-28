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
        public byte CompanyId { get; set; }
        [Key]
        public short UserId { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short UserGroupId { get; set; }
    }
}
