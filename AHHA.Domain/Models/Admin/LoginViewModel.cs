using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Admin
{
    public class LoginViewModel
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
}
