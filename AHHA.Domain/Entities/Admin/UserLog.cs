using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Admin
{
    public class UserLog
    {
        [Key]
        public short UserId { get; set; }
        public bool IsLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public string Remarks { get; set; }
    }
}
