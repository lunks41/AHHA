using AHHA.API.Controllers.Masters;
using AHHA.Application.IServices.Masters;
using AHHA.Application.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using AHHA.Core.Common;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Services.Masters;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        //Load User Company List.
        //Parameter : UserId
        //http://118.189.194.191:8080/ahharestapiproject/ahha/userCompany/{UserId}

        [HttpGet, Route("GetCompany")]
        public async Task<ActionResult> GetCompany()
        {
            try
            {
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));
                string RegId = Request.Headers.TryGetValue("RegId", out StringValues regIdValue).ToString().Trim();

                if (UserId > 0 && RegId.Length > 0)
                {
                    var Companydata = await _companyService.GetCompanyListAsync(UserId);

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
