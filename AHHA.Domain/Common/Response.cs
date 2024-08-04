using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AHHA.Core.Common
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
