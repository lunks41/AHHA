namespace AHHA.Core.Models.Masters
{
    public class AccountSetupCategoryViewModel
    {
        public Int16 AccSetupCategoryId { get; set; }
        public string AccSetupCategoryCode { get; set; }
        public string AccSetupCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}