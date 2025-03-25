using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.GL
{
    public class GLContraDtViewModel
    {
        private DateTime _docaccountDate;
        private DateTime _docdueDate;
        public string ContraId { get; set; }
        public string ContraNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public string DocReferenceNo { get; set; }
        public string DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public Int16 DocCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal DocExhRate { get; set; }

        public string DocAcctDate
        {
            get { return DateHelperStatic.FormatDate(_docaccountDate); }
            set { _docaccountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string DocDueDate
        {
            get { return DateHelperStatic.FormatDate(_docdueDate); }
            set { _docdueDate = DateHelperStatic.ParseDBDate(value); }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CentDiff { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public byte EditVersion { get; set; }
    }
}