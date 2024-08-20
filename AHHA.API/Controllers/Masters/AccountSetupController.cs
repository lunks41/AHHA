using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class AccountSetupController : BaseController
    {
        private readonly IAccountSetupService _AccountSetupService;
        private readonly ILogger<AccountSetupController> _logger;

        public AccountSetupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountSetupController> logger, IAccountSetupService AccountSetupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountSetupService = AccountSetupService;
        }

        [HttpGet, Route("GetAccountSetup")]
        [Authorize]
        public async Task<ActionResult> GetAllAccountSetup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _AccountSetupService.GetAccountSetupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return Ok(cacheData);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
                    }
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

        [HttpGet, Route("GetAccountSetupbyid/{AccSetupId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupViewModel>> GetAccountSetupById(Int16 AccSetupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var AccountSetupViewModel = new AccountSetupViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"AccountSetup_{AccSetupId}", out AccountSetupViewModel? cachedProduct))
                        {
                            AccountSetupViewModel = cachedProduct;
                        }
                        else
                        {
                            AccountSetupViewModel = _mapper.Map<AccountSetupViewModel>(await _AccountSetupService.GetAccountSetupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupId, headerViewModel.UserId));

                            if (AccountSetupViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the AccountSetup with an expiration time of 10 minutes
                                _memoryCache.Set($"AccountSetup_{AccSetupId}", AccountSetupViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, AccountSetupViewModel);
                        //return Ok(AccountSetupViewModel);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<AccountSetupViewModel>> CreateAccountSetup(AccountSetupViewModel AccountSetup, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = AccountSetup.IsActive,
                                Remarks = AccountSetup.Remarks
                            };

                            var createdAccountSetup = await _AccountSetupService.AddAccountSetupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountSetupEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdAccountSetup);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<AccountSetupViewModel>> UpdateAccountSetup(Int16 AccSetupId, [FromBody] AccountSetupViewModel AccountSetup, [FromHeader] HeaderViewModel headerViewModel)
        {
            var AccountSetupViewModel = new AccountSetupViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, headerViewModel.UserId);

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
                                var AccountSetupToUpdate = await _AccountSetupService.GetAccountSetupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupId, headerViewModel.UserId);

                                if (AccountSetupToUpdate == null)
                                    return NotFound($"M_AccountSetup with Id = {AccSetupId} not found");
                            }

                            var AccountSetupEntity = new M_AccountSetup
                            {
                                AccSetupCode = AccountSetup.AccSetupCode,
                                AccSetupId = AccountSetup.AccSetupId,
                                AccSetupName = AccountSetup.AccSetupName,
                                AccSetupCategoryId = AccountSetup.AccSetupCategoryId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountSetup.IsActive,
                                Remarks = AccountSetup.Remarks
                            };

                            var sqlResponce = await _AccountSetupService.UpdateAccountSetupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountSetupEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<M_AccountSetup>> DeleteAccountSetup(Int16 AccSetupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountSetupToDelete = await _AccountSetupService.GetAccountSetupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupId, headerViewModel.UserId);

                            if (AccountSetupToDelete == null)
                                return NotFound($"M_AccountSetup with Id = {AccSetupId} not found");

                            var sqlResponce = await _AccountSetupService.DeleteAccountSetupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountSetupToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"AccountSetup_{AccSetupId}");
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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