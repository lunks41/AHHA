namespace AHHA.Core.Models.Account.AR
{
    public class ARCreditNoteViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ARCreditNoteViewModel> data { get; set; }
    }
}