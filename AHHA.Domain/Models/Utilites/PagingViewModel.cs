using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Models.Utilites
{
    public class PagingViewModel
    {
        public Int16 pageSize { get; set; }
        public Int16 pageNumber { get; set; }
        public string searchString { get; set; }
    }
}
