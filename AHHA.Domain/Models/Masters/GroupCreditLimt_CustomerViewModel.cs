namespace AHHA.Core.Models.Masters
{
    public class GroupCreditLimt_CustomerViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int32 GroupCreditLimitId { get; set; }
        public int CustomerId { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}