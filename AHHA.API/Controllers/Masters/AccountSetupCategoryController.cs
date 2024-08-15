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
    public class AccountSetupCategoryController : BaseController
    {
        private readonly IAccountSetupCategoryService _AccountSetupCategoryService;
        private readonly ILogger<AccountSetupCategoryController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public AccountSetupCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountSetupCategoryController> logger, IAccountSetupCategoryService AccountSetupCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountSetupCategoryService = AccountSetupCategoryService;
        }

        [HttpGet, Route("GetAccountSetupCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllAccountSetupCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<AccountSetupCategoryViewModelCount>("AccountSetupCategory");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _AccountSetupCategoryService.GetAccountSetupCategoryListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<AccountSetupCategoryViewModelCount>("AccountSetupCategory", cacheData, expirationTime);

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

        [HttpGet, Route("GetAccountSetupCategorybyid/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> GetAccountSetupCategoryById(Int16 AccSetupCategoryId)
        {
            var AccountSetupCategoryViewModel = new AccountSetupCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"AccountSetupCategory_{AccSetupCategoryId}", out AccountSetupCategoryViewModel? cachedProduct))
                        {
                            AccountSetupCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            AccountSetupCategoryViewModel = _mapper.Map<AccountSetupCategoryViewModel>(await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(CompanyId, AccSetupCategoryId, UserId));

                            if (AccountSetupCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the AccountSetupCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"AccountSetupCategory_{AccSetupCategoryId}", AccountSetupCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, AccountSetupCategoryViewModel);
                        //return Ok(AccountSetupCategoryViewModel);
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

        [HttpPost, Route("AddAccountSetupCategory")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> CreateAccountSetupCategory(AccountSetupCategoryViewModel AccountSetupCategory)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (AccountSetupCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetupCategory ID mismatch");

                            var AccountSetupCategoryEntity = new M_AccountSetupCategory
                            {
                                AccSetupCategoryId = AccountSetupCategory.AccSetupCategoryId,
                                AccSetupCategoryCode = AccountSetupCategory.AccSetupCategoryCode,
                                AccSetupCategoryName = AccountSetupCategory.AccSetupCategoryName,
                                CreateById = UserId,
                                IsActive = AccountSetupCategory.IsActive,
                                Remarks = AccountSetupCategory.Remarks
                            };

                            var createdAccountSetupCategory = await _AccountSetupCategoryService.AddAccountSetupCategoryAsync(CompanyId, AccountSetupCategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdAccountSetupCategory);

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
                    "Error creating new AccountSetupCategory record");
            }
        }

        [HttpPut, Route("UpdateAccountSetupCategory/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> UpdateAccountSetupCategory(Int16 AccSetupCategoryId, [FromBody] AccountSetupCategoryViewModel AccountSetupCategory)
        {
            var AccountSetupCategoryViewModel = new AccountSetupCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AccSetupCategoryId != AccountSetupCategory.AccSetupCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetupCategory ID mismatch");
                            //return BadRequest("M_AccountSetupCategory ID mismatch");

                            // Attempt to retrieve the AccountSetupCategory from the cache
                            if (_memoryCache.TryGetValue($"AccountSetupCategory_{AccSetupCategoryId}", out AccountSetupCategoryViewModel? cachedProduct))
                            {
                                AccountSetupCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var AccountSetupCategoryToUpdate = await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(CompanyId, AccSetupCategoryId, UserId);

                                if (AccountSetupCategoryToUpdate == null)
                                    return NotFound($"M_AccountSetupCategory with Id = {AccSetupCategoryId} not found");
                            }

                            var AccountSetupCategoryEntity = new M_AccountSetupCategory
                            {
                                AccSetupCategoryCode = AccountSetupCategory.AccSetupCategoryCode,
                                AccSetupCategoryId = AccountSetupCategory.AccSetupCategoryId,
                                AccSetupCategoryName = AccountSetupCategory.AccSetupCategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountSetupCategory.IsActive,
                                Remarks = AccountSetupCategory.Remarks
                            };

                            var sqlResponce = await _AccountSetupCategoryService.UpdateAccountSetupCategoryAsync(CompanyId, AccountSetupCategoryEntity, UserId);
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

        [HttpDelete, Route("Delete/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_AccountSetupCategory>> DeleteAccountSetupCategory(Int16 AccSetupCategoryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountSetupCategoryToDelete = await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(CompanyId, AccSetupCategoryId, UserId);

                            if (AccountSetupCategoryToDelete == null)
                                return NotFound($"M_AccountSetupCategory with Id = {AccSetupCategoryId} not found");

                            var sqlResponce = await _AccountSetupCategoryService.DeleteAccountSetupCategoryAsync(CompanyId, AccountSetupCategoryToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"AccountSetupCategory_{AccSetupCategoryId}");
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

