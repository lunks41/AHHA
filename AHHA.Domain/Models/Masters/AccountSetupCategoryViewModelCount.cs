﻿namespace AHHA.Core.Models.Masters
{
    public class AccountSetupCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountSetupCategoryViewModel> data { get; set; }
    }
}