namespace AHHA.Core.Models.Admin
{
    public class UserLogViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<UserLogViewModel> data { get; set; }
    }
}