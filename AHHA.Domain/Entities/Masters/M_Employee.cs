using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Masters
{
    public class M_Employee
    {
        [Key]
        public short EmployeeId { get; set; }
        public byte CompanyId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeOtherName { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmployeeSignature { get; set; }
        public short DepartmentId { get; set; }
        public string EmployeeSex { get; set; }
        public string MartialStatus { get; set; }
        public DateTime EmployeeDOB { get; set; }
        public DateTime EmployeeJoinDate { get; set; }
        public DateTime EmployeeLastDate { get; set; }
        public string EmployeeOffEmailAdd { get; set; }
        public string EmployeeOtherEmailAdd { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short EditById { get; set; }
        public DateTime EditDate { get; set; }
    }
}
