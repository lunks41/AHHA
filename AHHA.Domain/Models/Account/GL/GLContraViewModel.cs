namespace AHHA.Core.Models.Account.GL
{
    public class GLContraViewModel
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GLContraHdViewModel> data { get; set; }
    }
}