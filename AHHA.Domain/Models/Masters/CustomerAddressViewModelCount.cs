﻿namespace AHHA.Core.Models.Masters
{
    public class CustomerAddressViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CustomerAddressViewModel> data { get; set; }
    }
}