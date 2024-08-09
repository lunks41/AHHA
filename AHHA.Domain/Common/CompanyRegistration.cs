using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Common
{
    public class CompanyRegistration
    {
        public Int16 RegId { get; set; }
        public string CompanyName { get; set; }
        public string ConnectionStringName { get; set; }
    }
}
