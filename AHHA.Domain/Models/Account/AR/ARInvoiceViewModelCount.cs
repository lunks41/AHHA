﻿namespace AHHA.Core.Models.Account.AR
{
    public class ARInvoiceViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ARInvoiceViewModel> data { get; set; }
    }
}