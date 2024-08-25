﻿using AHHA.Application.IServices;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers
{
    [Route("api/Master")]
    [ApiController]
    public class MasterLookupController : BaseController
    {
        private readonly IMasterLookupService _MasterLookupService;
        private readonly ILogger<MasterLookupController> _logger;

        public MasterLookupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<MasterLookupController> logger, IMasterLookupService MasterLookupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _MasterLookupService = MasterLookupService;
        }

        [HttpGet, Route("GetAccountSetupCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> AccountSetupCategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetAccountSetupCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetAccountSetupLookup")]
        [Authorize]
        public async Task<ActionResult> AccountSetupLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetAccountSetupLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCountryLookup")]
        [Authorize]
        public async Task<ActionResult> CountryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCountryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetVesselLookup")]
        [Authorize]
        public async Task<ActionResult> VesselLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetVesselLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetBargeLookup")]
        [Authorize]
        public async Task<ActionResult> BargeLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetBargeLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> CategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetBankLookup")]
        [Authorize]
        public async Task<ActionResult> BankLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetBankLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetChartOfAccountLookup")]
        [Authorize]
        public async Task<ActionResult> ChartOfAccountLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetChartOfAccountLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCOACategory1Lookup")]
        [Authorize]
        public async Task<ActionResult> COACategory1Lookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCOACategory1LookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCOACategory2Lookup")]
        [Authorize]
        public async Task<ActionResult> COACategory2Lookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCOACategory2LookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCOACategory3Lookup")]
        [Authorize]
        public async Task<ActionResult> COACategory3Lookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCOACategory3LookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCustomerGroupCreditLimitLookup")]
        [Authorize]
        public async Task<ActionResult> CustomerGroupCreditLimitLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCustomerGroupCreditLimitLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCustomerLookup")]
        [Authorize]
        public async Task<ActionResult> CustomerLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCustomerLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetDepartmentLookup")]
        [Authorize]
        public async Task<ActionResult> DepartmentLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetDepartmentLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetEmployeeLookup")]
        [Authorize]
        public async Task<ActionResult> EmployeeLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetEmployeeLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetGroupCreditLimitLookup")]
        [Authorize]
        public async Task<ActionResult> GroupCreditLimitLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetGroupCreditLimitLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetGstLookup")]
        [Authorize]
        public async Task<ActionResult> GstLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetGstLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetGstCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> GstCategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetGstCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetOrderTypeLookup")]
        [Authorize]
        public async Task<ActionResult> OrderTypeLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetOrderTypeLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetOrderTypeCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> OrderTypeCategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetOrderTypeCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetPaymentTypeLookup")]
        [Authorize]
        public async Task<ActionResult> PaymentTypeLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetPaymentTypeLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetPortLookup")]
        [Authorize]
        public async Task<ActionResult> PortLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetPortLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetPortRegionLookup")]
        [Authorize]
        public async Task<ActionResult> PortRegionLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetPortRegionLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetProductLookup")]
        [Authorize]
        public async Task<ActionResult> ProductLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetProductLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetSubCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> SubCategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetSubCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetSupplierLookup")]
        [Authorize]
        public async Task<ActionResult> SupplierLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetSupplierLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetUomLookup")]
        [Authorize]
        public async Task<ActionResult> UomLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetUomLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetVoyageLookup")]
        [Authorize]
        public async Task<ActionResult> VoyageLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetVoyageLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
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