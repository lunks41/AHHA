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
    public class BankController : BaseController
    {
        private readonly IBankService _BankService;
        private readonly ILogger<BankController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public BankController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BankController> logger, IBankService BankService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BankService = BankService;
        }

        [HttpGet, Route("GetBank")]
        [Authorize]
        public async Task<ActionResult> GetAllBank()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<BankViewModelCount>("Bank");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _BankService.GetBankListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<BankViewModelCount>("Bank", cacheData, expirationTime);

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

        [HttpGet, Route("GetBankbyid/{BankId}")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> GetBankById(Int16 BankId)
        {
            var BankViewModel = new BankViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Bank_{BankId}", out BankViewModel? cachedProduct))
                        {
                            BankViewModel = cachedProduct;
                        }
                        else
                        {
                            BankViewModel = _mapper.Map<BankViewModel>(await _BankService.GetBankByIdAsync(CompanyId, BankId, UserId));

                            if (BankViewModel == null)
                                return NotFound();
                            else
                                // Cache the Bank with an expiration time of 10 minutes
                                _memoryCache.Set($"Bank_{BankId}", BankViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, BankViewModel);
                        //return Ok(BankViewModel);
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

        [HttpPost, Route("AddBank")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> CreateBank(BankViewModel Bank)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Bank == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Bank ID mismatch");

                            var BankEntity = new M_Bank
                            {
                                CompanyId = Bank.CompanyId,
                                BankCode = Bank.BankCode,
                                BankId = Bank.BankId,
                                BankName = Bank.BankName,
                                CurrencyId = Bank.CurrencyId,
                                AccountNo = Bank.AccountNo,
                                SwiftCode = Bank.SwiftCode,
                                CreateById = UserId,
                                IsActive = Bank.IsActive,
                                Remarks1 = Bank.Remarks1,
                                Remarks2 = Bank.Remarks2,
                            };

                            var createdBank = await _BankService.AddBankAsync(CompanyId, BankEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdBank);

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
                    "Error creating new Bank record");
            }
        }

        [HttpPut, Route("UpdateBank/{BankId}")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> UpdateBank(Int16 BankId, [FromBody] BankViewModel Bank)
        {
            var BankViewModel = new BankViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (BankId != Bank.BankId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Bank ID mismatch");
                            //return BadRequest("M_Bank ID mismatch");

                            // Attempt to retrieve the Bank from the cache
                            if (_memoryCache.TryGetValue($"Bank_{BankId}", out BankViewModel? cachedProduct))
                            {
                                BankViewModel = cachedProduct;
                            }
                            else
                            {
                                var BankToUpdate = await _BankService.GetBankByIdAsync(CompanyId, BankId, UserId);

                                if (BankToUpdate == null)
                                    return NotFound($"M_Bank with Id = {BankId} not found");
                            }

                            var BankEntity = new M_Bank
                            {
                                CompanyId = Bank.CompanyId,
                                BankCode = Bank.BankCode,
                                BankId = Bank.BankId,
                                BankName = Bank.BankName,
                                CurrencyId = Bank.CurrencyId,
                                AccountNo = Bank.AccountNo,
                                SwiftCode = Bank.SwiftCode,
                                IsActive = Bank.IsActive,
                                Remarks1 = Bank.Remarks1,
                                Remarks2 = Bank.Remarks2,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _BankService.UpdateBankAsync(CompanyId, BankEntity, UserId);
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

        [HttpDelete, Route("Delete/{BankId}")]
        [Authorize]
        public async Task<ActionResult<M_Bank>> DeleteBank(Int16 BankId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var BankToDelete = await _BankService.GetBankByIdAsync(CompanyId, BankId, UserId);

                            if (BankToDelete == null)
                                return NotFound($"M_Bank with Id = {BankId} not found");

                            var sqlResponce = await _BankService.DeleteBankAsync(CompanyId, BankToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Bank_{BankId}");
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
