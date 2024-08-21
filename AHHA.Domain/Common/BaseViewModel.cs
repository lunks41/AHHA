using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Common
{
    public class BaseViewModel<T>
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
    }
}
