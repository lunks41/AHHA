namespace AHHA.Core.Models.Masters
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public short CompanyId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public int CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}
