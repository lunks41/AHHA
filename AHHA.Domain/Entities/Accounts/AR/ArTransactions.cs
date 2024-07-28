using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArTransactions
    {
        public byte CompanyId { get; set; }
        public byte ModuleId { get; set; }
        public byte TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CustomerId { get; set; }
        public byte CurrencyId { get; set; }
        public byte RefModuleId { get; set; }
        public byte RefTransactionId { get; set; }
        public long RefDocumentId { get; set; }
        public string RefDocumentNo { get; set; }
        public string RefReferenceNo { get; set; }
        public DateTime RefAccountDate { get; set; }
        public int RefCustomerId { get; set; }
        public byte RefCurrencyId { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal AllAmt { get; set; }
        public decimal AllLocalAmt { get; set; }
        public decimal ExGainLoss { get; set; }
    }
}
