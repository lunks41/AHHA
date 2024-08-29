namespace AHHA.Core.Models.Admin
{
    public class UserGroupRightsViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<UserGroupRightsViewModel> data { get; set; }
    }
}