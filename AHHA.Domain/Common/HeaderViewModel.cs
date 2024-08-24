﻿using Microsoft.AspNetCore.Mvc;

namespace AHHA.Core.Common
{
    public class HeaderViewModel
    {
        private string? _searchStr;

        [FromHeader]
        public string RegId { get; set; }

        [FromHeader]
        public Int16 CompanyId { get; set; }

        [FromHeader]
        public Int32 UserId { get; set; }

        [FromHeader]
        public Int16 pageSize { get; set; }

        [FromHeader]
        public Int16 pageNumber { get; set; }

        [FromHeader]
        public string searchString { get { return _searchStr.Trim() == null ? string.Empty : _searchStr; } set { _searchStr = value; } }

        [FromHeader]
        public Int16 ModuleId { get; set; }
    }
}