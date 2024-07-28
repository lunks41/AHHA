using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Setting
{
    public class S_UserGrdFormat
    {
        public byte CompanyId { get; set; }
        public short UserId { get; set; }
        public byte ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string GrdName { get; set; }
        public string GrdString { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
