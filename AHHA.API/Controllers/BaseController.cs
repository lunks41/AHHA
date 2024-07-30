using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace AHHA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public BaseController() { }

        public bool ValidateHeaders(Int16 CompanyId, Int32 UserId)
        {
            bool IsValidate = false;

            if (CompanyId > 0 && UserId > 0)
            {
                IsValidate = true;
            }
            return IsValidate;
        }
    }
}
