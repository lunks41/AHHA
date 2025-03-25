using AHHA.Core.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AHHA.Core.Models.Account.GL
{
    public class GLContraHdViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _chequeDate;
        private DateTime _gstClaimDate;
        private DateTime _revDate;
        private DateTime _revcurrenceUntilDate;
        public Int16 CompanyId { get; set; }

        public string ContraId { get; set; }

        public string ContraNo { get; set; }
        public string ReferenceNo { get; set; }

        public string TrnDate
        {
            get { return DateHelperStatic.FormatDate(_trnDate); }
            set { _trnDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public Int32 CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public Int32 SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

        public Int16 CurrencyId { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public string CancelBy { get; set; }
        public byte EditVersion { get; set; }
        public List<GLContraDtViewModel> data_details { get; set; }
    }
}