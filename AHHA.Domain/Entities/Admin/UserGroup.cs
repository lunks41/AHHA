using System.ComponentModel.DataAnnotations;

namespace AHHA.Core.Entities.Admin
{
    public class UserGroup
    {
        [Key]
        public Int32 UserGroupId { get; set; }

        public string UserGroupCode { get; set; }
        public string UserGroupName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}