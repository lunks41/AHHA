﻿namespace AHHA.Core.Models.Masters
{
    public class CustomerGroupCreditLimitViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CustomerGroupCreditLimitViewModel> data { get; set; }
    }
}