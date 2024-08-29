namespace AHHA.Core.Models.Admin
{
    public class UserLogViewModel
    {
        public Int32 UserId { get; set; }
        public bool IsLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public string Remarks { get; set; }
    }
}