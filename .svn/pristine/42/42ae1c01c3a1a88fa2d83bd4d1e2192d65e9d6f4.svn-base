using AHHA.Core.Helper;

namespace AHHA.Core.Models.Account.GL
{
    public class GLPeriodCloseViewModel
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime _arCloseDate;
        private DateTime _apCloseDate;
        private DateTime _cbCloseDate;
        private DateTime _glCloseDate;
        public short CompnyId { get; set; }
        public int FinYear { get; set; }
        public short FinMonth { get; set; }

        public string StartDate
        {
            get { return DateHelperStatic.FormatDate(_startDate); }
            set { _startDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string EndDate
        {
            get { return DateHelperStatic.FormatDate(_endDate); }
            set { _endDate = DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsArClose { get; set; }
        public int ArCloseById { get; set; }

        public string ArCloseDate
        {
            get { return DateHelperStatic.FormatDate(_arCloseDate); }
            set { _arCloseDate = DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsApClose { get; set; }
        public int ApCloseById { get; set; }

        public string ApCloseDate
        {
            get { return DateHelperStatic.FormatDate(_apCloseDate); }
            set { _apCloseDate = DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsCbClose { get; set; }
        public int CbCloseById { get; set; }

        public string CbCloseDate
        {
            get { return DateHelperStatic.FormatDate(_cbCloseDate); }
            set { _cbCloseDate = DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsGlClose { get; set; }
        public int GlCloseById { get; set; }

        public string GlCloseDate
        {
            get { return DateHelperStatic.FormatDate(_glCloseDate); }
            set { _glCloseDate = DateHelperStatic.ParseDBDate(value); }
        }

        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}