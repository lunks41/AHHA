using Microsoft.AspNetCore.Mvc;

namespace AHHA.Core.Common
{
    public class HeaderViewModel
    {
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
        public string searchString { get; set; }

        [FromHeader]
        public Int16 ModuleId { get; set; }
    }
}