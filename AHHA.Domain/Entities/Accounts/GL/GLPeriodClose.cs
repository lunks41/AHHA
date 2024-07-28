using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLPeriodClose
    {
        public byte CompnyId { get; set; }
        public short FinYear { get; set; }
        public byte FinMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsArClose { get; set; }
        public short ArCloseById { get; set; }
        public DateTime ArCloseDate { get; set; }
        public bool IsApClose { get; set; }
        public short ApCloseById { get; set; }
        public DateTime ApCloseDate { get; set; }
        public bool IsCbClose { get; set; }
        public short CbCloseById { get; set; }
        public DateTime CbCloseDate { get; set; }
        public bool IsGlClose { get; set; }
        public short GlCloseById { get; set; }
        public DateTime GlCloseDate { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
