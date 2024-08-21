namespace AHHA.Core.Models.Masters
{
    public class GroupCreditLimtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<GroupCreditLimtViewModel> data { get; set; }
    }
}