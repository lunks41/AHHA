using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Entities.Accounts.AR
{
    public class ArOutstandTransactions
    {
        public byte CompnyId { get; set; }
        public byte TransactionId { get; set; }
        public long DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CustomerId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal ExhRate { get; set; }
        public decimal TotAmt { get; set; }
        public decimal TotLocalAmt { get; set; }
        public decimal BalAmt { get; set; }
        public decimal BalLocalAmt { get; set; }
        public string Remarks { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
