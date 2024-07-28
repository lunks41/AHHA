namespace AHHA.API.Models
{
    public class CountryViewModel
    {
        public short CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public byte CompanyId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }
}
