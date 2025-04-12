﻿using AHHA.Core.Helper;

namespace AHHA.Core.Models.Masters
{
    public class EmployeeViewModel
    {
        private DateTime _employeeDOB;
        private DateTime _employeeJoinDate;
        private DateTime _employeeLastDate;

        public Int16 EmployeeId { get; set; }
        public Int16 CompanyId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeOtherName { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmployeeSignature { get; set; }
        public Int16 DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeSex { get; set; }
        public string MartialStatus { get; set; }

        public string EmployeeDOB
        {
            get { return DateHelperStatic.FormatDate(_employeeDOB); }
            set { _employeeDOB = DateHelperStatic.ParseDBDate(value); }
        }

        public string EmployeeJoinDate
        {
            get { return DateHelperStatic.FormatDate(_employeeJoinDate); }
            set { _employeeJoinDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string EmployeeLastDate
        {
            get { return DateHelperStatic.FormatDate(_employeeLastDate); }
            set { _employeeLastDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string EmployeeOffEmailAdd { get; set; }
        public string EmployeeOtherEmailAdd { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}