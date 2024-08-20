using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Admin
{
    [Keyless]
    public class AdmUserRights
    {
        [Key]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int32 UserId { get; set; }

        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 UserGroupId { get; set; }
    }
}