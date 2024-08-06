using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models
{
    public class LoginResponse
    {
        //public bool IsLogedIn { get; set; } = false;
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        //public DateTime? Expiration { get; set; }
    }
}
