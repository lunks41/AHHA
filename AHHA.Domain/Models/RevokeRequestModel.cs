﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models
{
    public class RevokeRequestModel
    {
        public string RefreshToken { get; set; }
        public string JwtToken { get; set; }
    }
}
