using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.GL
{
    public class GLPeriodClose
    {
        public Int16 CompnyId { get; set; }
        public Int32 FinYear { get; set; }
        public Int16 FinMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsArClose { get; set; }
        public Int32 ArCloseById { get; set; }
        public DateTime ArCloseDate { get; set; }
        public bool IsApClose { get; set; }
        public Int32 ApCloseById { get; set; }
        public DateTime ApCloseDate { get; set; }
        public bool IsCbClose { get; set; }
        public Int32 CbCloseById { get; set; }
        public DateTime CbCloseDate { get; set; }
        public bool IsGlClose { get; set; }
        public Int32 GlCloseById { get; set; }
        public DateTime GlCloseDate { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
