using AHHA.Application.IServices.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;
        private Int32 UserId = 0;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        //Load User Company List.
        //Parameter : UserId
        //http://118.189.194.191:8080/ahharestapiproject/ahha/userCompany/{UserId}

        [HttpGet, Route("GetUserCompany")]
        public async Task<ActionResult> GetUserCompany()
        {
            try
            {
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                string RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (UserId > 0 && RegId.Length > 0)
                {
                    var Companydata = await _companyService.GetUserCompanyListAsync(UserId);

                    return Ok(Companydata);
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (RegId.Length == 0)
                        return NotFound("RegistrationId Not Found");
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }
    }
}
