namespace AHHA.Core.Models.Account.AR
{
    public class ARDebitNoteViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ARDebitNoteViewModel> data { get; set; }
    }
}