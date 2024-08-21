﻿namespace AHHA.Core.Models.Masters
{
    public class BargeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<BargeViewModel> data { get; set; }
    }
}