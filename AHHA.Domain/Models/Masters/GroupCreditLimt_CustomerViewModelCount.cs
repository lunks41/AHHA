namespace AHHA.Core.Models.Masters
{
    public class GroupCreditLimt_CustomerViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<GroupCreditLimt_CustomerViewModel> data { get; set; }
    }
}