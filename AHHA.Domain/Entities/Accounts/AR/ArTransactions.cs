namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArTransactions
    {
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CustomerId { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 RefModuleId { get; set; }
        public Int16 RefTransactionId { get; set; }
        public long RefDocumentId { get; set; }
        public string RefDocumentNo { get; set; }
        public string RefReferenceNo { get; set; }
        public DateTime RefAccountDate { get; set; }
        public int RefCustomerId { get; set; }
        public Int16 RefCurrencyId { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal AllAmt { get; set; }
        public decimal AllLocalAmt { get; set; }
        public decimal ExGainLoss { get; set; }
    }
}