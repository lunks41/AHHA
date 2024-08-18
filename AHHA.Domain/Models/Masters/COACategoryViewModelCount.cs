using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Masters
{
    public class COACategoryViewModelCount
    {
        public long totalRecords { get; set; }
        public List<COACategoryViewModel> data { get; set; }
    }
}
