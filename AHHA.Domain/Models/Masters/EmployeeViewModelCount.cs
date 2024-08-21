namespace AHHA.Core.Models.Masters
{
    public class EmployeeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<EmployeeViewModel> data { get; set; }
    }
}