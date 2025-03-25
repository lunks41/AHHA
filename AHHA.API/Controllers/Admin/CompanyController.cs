﻿using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using Microsoft.AspNetCore.Mvc;

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
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal serever error");
            }
        }
    }
}