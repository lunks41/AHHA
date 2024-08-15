using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Masters
{
    public class COACategoryViewModelCount
    {
        public long Total_records { get; set; }
        public List<COACategoryViewModel> COACategoryViewModels { get; set; }
    }
}
