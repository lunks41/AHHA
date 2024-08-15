namespace AHHA.Core.Models.Masters
{
    public class CustomerCreditLimitViewModel
    {
        public int CustomerId { get; set; }
        public Int16 CompanyId { get; set; }
        public DateTime EffectFrom { get; set; }
        public DateTime EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public decimal CreditLimitAmt { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}
