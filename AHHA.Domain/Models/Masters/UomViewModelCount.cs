﻿namespace AHHA.Core.Models.Masters
{
    public class UomViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UomViewModel> data { get; set; }
    }
}