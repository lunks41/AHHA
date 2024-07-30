using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArDocSetOffHd
    {
        public Int16 CompanyId { get; set; }
        public long SetoffId { get; set; }
        public string SetoffNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public int CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public string Remarks { get; set; }
        public decimal AllocAmt { get; set; }
        public decimal UnAllocAmt { get; set; }
        public decimal ExhGainLoss { get; set; }
        public string ModuleFrom { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16 EditVer { get; set; }
        public Int32 EditById { get; set; }
        public DateTime EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int32 CancelById { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelRemarks { get; set; }
    }
}
