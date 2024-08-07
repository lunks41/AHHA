using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Admin
{
    public class ModuleViewModel
    {
        public Int16 CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
