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
    public class BankController : BaseController
    {
        private readonly IBankService _BankService;
        private readonly ILogger<BankController> _logger;

        public BankController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BankController> logger, IBankService BankService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BankService = BankService;
        }

        [HttpGet, Route("GetBank")]
        [Authorize]
        public async Task<ActionResult> GetBank([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var cacheData = await _BankService.GetBankListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetBankbyid/{BankId}")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> GetBankById(Int16 BankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var BankViewModel = new BankViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Bank_{BankId}", out BankViewModel? cachedProduct))
                        {
                            BankViewModel = cachedProduct;
                        }
                        else
                        {
                            BankViewModel = _mapper.Map<BankViewModel>(await _BankService.GetBankByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId));

                            if (BankViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Bank with an expiration time of 10 minutes
                                _memoryCache.Set($"Bank_{BankId}", BankViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, BankViewModel);
                        //return Ok(BankViewModel);
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

        [HttpPost, Route("AddBank")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> CreateBank(BankViewModel Bank, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = Bank.IsActive,
                                Remarks1 = Bank.Remarks1,
                                Remarks2 = Bank.Remarks2,
                            };

                            var createdBank = await _BankService.AddBankAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdBank);
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
                    "Error creating new Bank record");
            }
        }

        [HttpPut, Route("UpdateBank/{BankId}")]
        [Authorize]
        public async Task<ActionResult<BankViewModel>> UpdateBank(Int16 BankId, [FromBody] BankViewModel Bank, [FromHeader] HeaderViewModel headerViewModel)
        {
            var BankViewModel = new BankViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, headerViewModel.UserId);

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
                                var BankToUpdate = await _BankService.GetBankByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId);

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
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _BankService.UpdateBankAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteBank/{BankId}")]
        [Authorize]
        public async Task<ActionResult<M_Bank>> DeleteBank(Int16 BankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var BankToDelete = await _BankService.GetBankByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId);

                            if (BankToDelete == null)
                                return NotFound($"M_Bank with Id = {BankId} not found");

                            var sqlResponce = await _BankService.DeleteBankAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Bank_{BankId}");
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