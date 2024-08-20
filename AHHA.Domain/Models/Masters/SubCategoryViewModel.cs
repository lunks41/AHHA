namespace AHHA.Core.Models.Masters
{
    public class SubCategoryViewModel
    {
        public int SubCategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
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