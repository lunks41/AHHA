using Microsoft.EntityFrameworkCore;

namespace AHHA.Core.Entities.Admin
{
    [Keyless]
    public class ShareData
    {
        public byte ModuleId { get; set; }
        public short TransactionId { get; set; }
        public bool ShareToAll { get; set; }
        public byte CompanyId { get; set; }
        public byte SetId { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
    }
}
