﻿using AHHA.Core.Helper;
using Newtonsoft.Json.Linq;

namespace AHHA.Core.Models.Account.GL
{
    public class GLPeriodCloseViewModel
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime? _arCloseDate;
        private DateTime? _apCloseDate;
        private DateTime? _cbCloseDate;
        private DateTime? _glCloseDate;
        public Int16 CompanyId { get; set; }
        public Int32 FinYear { get; set; }
        public Int16 FinMonth { get; set; }

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
        public Int32 ArCloseById { get; set; }

        public string ArCloseDate
        {
            get { return _arCloseDate.HasValue ? DateHelperStatic.FormatDate(_arCloseDate.Value) : ""; }
            set { _arCloseDate = string.IsNullOrEmpty(value) ? (DateTime?)null : DateHelperStatic.ParseDBDate(value); }
        }

        //get { return DateHelperStatic.FormatDate(_arCloseDate); }
        //set { _arCloseDate = DateHelperStatic.ParseDBDate(value); }

        public bool IsApClose { get; set; }
        public Int32 ApCloseById { get; set; }

        public string ApCloseDate
        {
            get { return _apCloseDate.HasValue ? DateHelperStatic.FormatDate(_apCloseDate.Value) : ""; }
            set { _apCloseDate = string.IsNullOrEmpty(value) ? (DateTime?)null : DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsCbClose { get; set; }
        public Int32 CbCloseById { get; set; }

        public string CbCloseDate
        {
            get { return _cbCloseDate.HasValue ? DateHelperStatic.FormatDate(_cbCloseDate.Value) : ""; }
            set { _cbCloseDate = string.IsNullOrEmpty(value) ? (DateTime?)null : DateHelperStatic.ParseDBDate(value); }
        }

        public bool IsGlClose { get; set; }
        public Int32 GlCloseById { get; set; }

        public string GlCloseDate
        {
            get { return _glCloseDate.HasValue ? DateHelperStatic.FormatDate(_glCloseDate.Value) : ""; }
            set { _glCloseDate = string.IsNullOrEmpty(value) ? (DateTime?)null : DateHelperStatic.ParseDBDate(value); }
        }

        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public string ArCloseBy { get; set; }
        public string ApCloseBy { get; set; }
        public string CbCloseBy { get; set; }
        public string GlCloseBy { get; set; }
    }

    public class PeriodCloseViewModel
    {
        public Int32 FinYear { get; set; }
        public Int16 FinMonth { get; set; }
        public string FieldName { get; set; }
        public bool IsValue { get; set; }
    }
}