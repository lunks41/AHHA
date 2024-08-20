using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
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

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        //Load User Company List.
        //Parameter : UserId
        //http://118.189.194.191:8080/ahharestapiproject/ahha/userCompany/{UserId}

        [HttpGet, Route("GetUserCompany")]
        public async Task<ActionResult> GetUserCompany([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (headerViewModel.UserId > 0 && headerViewModel.RegId != "")
                {
                    var Companydata = await _companyService.GetUserCompanyListAsync(headerViewModel.RegId, headerViewModel.UserId);

                    return Ok(Companydata);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,"Internal serever error");
            }
        }
    }
}
