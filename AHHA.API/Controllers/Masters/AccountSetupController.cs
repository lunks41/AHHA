using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class AccountSetupController : BaseController
    {
        private readonly IAccountSetupService _AccountSetupService;
        private readonly ILogger<AccountSetupController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public AccountSetupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountSetupController> logger, IAccountSetupService AccountSetupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountSetupService = AccountSetupService;
        }

        [HttpGet, Route("GetAccountSetup")]
        [Authorize]
        public async Task<ActionResult> GetAllAccountSetup()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<AccountSetupViewModelCount>("AccountSetup");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _AccountSetupService.GetAccountSetupListAsync(RegId, CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<AccountSetupViewModelCount>("AccountSetup", cacheData, expirationTime);

                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                            //return Ok(cacheData);
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (CompanyId == 0)
                        return NotFound("CompanyId Not Found");
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

        [HttpGet, Route("GetAccountSetupbyid/{AccSetupId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupViewModel>> GetAccountSetupById(Int16 AccSetupId)
        {
            var AccountSetupViewModel = new AccountSetupViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"AccountSetup_{AccSetupId}", out AccountSetupViewModel? cachedProduct))
                        {
                            AccountSetupViewModel = cachedProduct;
                        }
                        else
                        {
                            AccountSetupViewModel = _mapper.Map<AccountSetupViewModel>(await _AccountSetupService.GetAccountSetupByIdAsync(RegId, CompanyId, AccSetupId, UserId));

                            if (AccountSetupViewModel == null)
                                return NotFound();
                            else
                                // Cache the AccountSetup with an expiration time of 10 minutes
                                _memoryCache.Set($"AccountSetup_{AccSetupId}", AccountSetupViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, AccountSetupViewModel);
                        //return Ok(AccountSetupViewModel);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("AddAccountSetup")]
        [Authorize]
        public async Task<ActionResult<AccountSetupViewModel>> CreateAccountSetup(AccountSetupViewModel AccountSetup)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (AccountSetup == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetup ID mismatch");

                            var AccountSetupEntity = new M_AccountSetup
                            {
                                CompanyId = AccountSetup.CompanyId,
                                AccSetupCode = AccountSetup.AccSetupCode,
                                AccSetupId = AccountSetup.AccSetupId,
                                AccSetupName = AccountSetup.AccSetupName,
                                AccSetupCategoryId = AccountSetup.AccSetupCategoryId,
                                CreateById = UserId,
                                IsActive = AccountSetup.IsActive,
                                Remarks = AccountSetup.Remarks
                            };

                            var createdAccountSetup = await _AccountSetupService.AddAccountSetupAsync(RegId, CompanyId, AccountSetupEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdAccountSetup);

                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new AccountSetup record");
            }
        }

        [HttpPut, Route("UpdateAccountSetup/{AccSetupId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupViewModel>> UpdateAccountSetup(Int16 AccSetupId, [FromBody] AccountSetupViewModel AccountSetup)
        {
            var AccountSetupViewModel = new AccountSetupViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AccSetupId != AccountSetup.AccSetupId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetup ID mismatch");
                            //return BadRequest("M_AccountSetup ID mismatch");

                            // Attempt to retrieve the AccountSetup from the cache
                            if (_memoryCache.TryGetValue($"AccountSetup_{AccSetupId}", out AccountSetupViewModel? cachedProduct))
                            {
                                AccountSetupViewModel = cachedProduct;
                            }
                            else
                            {
                                var AccountSetupToUpdate = await _AccountSetupService.GetAccountSetupByIdAsync(RegId, CompanyId, AccSetupId, UserId);

                                if (AccountSetupToUpdate == null)
                                    return NotFound($"M_AccountSetup with Id = {AccSetupId} not found");
                            }

                            var AccountSetupEntity = new M_AccountSetup
                            {
                                AccSetupCode = AccountSetup.AccSetupCode,
                                AccSetupId = AccountSetup.AccSetupId,
                                AccSetupName = AccountSetup.AccSetupName,
                                AccSetupCategoryId = AccountSetup.AccSetupCategoryId,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountSetup.IsActive,
                                Remarks = AccountSetup.Remarks
                            };

                            var sqlResponce = await _AccountSetupService.UpdateAccountSetupAsync(RegId, CompanyId, AccountSetupEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete, Route("Delete/{AccSetupId}")]
        [Authorize]
        public async Task<ActionResult<M_AccountSetup>> DeleteAccountSetup(Int16 AccSetupId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountSetupToDelete = await _AccountSetupService.GetAccountSetupByIdAsync(RegId, CompanyId, AccSetupId, UserId);

                            if (AccountSetupToDelete == null)
                                return NotFound($"M_AccountSetup with Id = {AccSetupId} not found");

                            var sqlResponce = await _AccountSetupService.DeleteAccountSetupAsync(RegId, CompanyId, AccountSetupToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"AccountSetup_{AccSetupId}");
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}

