﻿namespace AHHA.Core.Models.Masters
{
    public class CountryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CountryViewModel> data { get; set; }
    }
}