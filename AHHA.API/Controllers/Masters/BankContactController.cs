﻿using AHHA.Application.IServices;
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
    public class BankContactController : BaseController
    {
        private readonly IBankContactService _BankContactService;
        private readonly ILogger<BankContactController> _logger;

        public BankContactController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BankContactController> logger, IBankContactService BankContactService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BankContactService = BankContactService;
        }

        //Get the Bank Contact List
        [HttpGet, Route("getbankcontactbybankid/{BankId}")]
        [Authorize]
        public async Task<ActionResult> GetBankContactByBankId(Int16 BankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var cacheData = await _BankContactService.GetBankContactByBankIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId);

                if (cacheData == null)
                    return Ok(new SqlResponse { Result = -1, Message = "failed", Data = null, TotalRecords = 0 });

                return Ok(new SqlResponse { Result = 1, Message = "success", Data = cacheData, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //Bank Contact one record by using contactid
        [HttpGet, Route("getbankcontactbyid/{BankId}/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<BankContactViewModel>> GetBankContactById(Int32 BankId, Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var cacheData = await _BankContactService.GetBankContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, ContactId, headerViewModel.UserId);

                if (cacheData == null)
                    return Ok(new SqlResponse { Result = -1, Message = "failed", Data = null, TotalRecords = 0 });

                return Ok(new SqlResponse { Result = 1, Message = "success", Data = cacheData, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveBankContact")]
        [Authorize]
        public async Task<ActionResult<BankContactViewModel>> SaveBankContact(BankContactViewModel bankContactViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (bankContactViewModel == null || bankContactViewModel.BankId == 0)
                    return NotFound();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null || !userGroupRight.IsCreate)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var BankContactEntity = new M_BankContact
                {
                    BankId = bankContactViewModel.BankId,
                    ContactId = bankContactViewModel.ContactId,
                    ContactName = bankContactViewModel.ContactName?.Trim() ?? string.Empty,
                    OtherName = bankContactViewModel.OtherName?.Trim() ?? string.Empty,
                    OffNo = bankContactViewModel.OffNo?.Trim() ?? string.Empty,
                    FaxNo = bankContactViewModel.FaxNo?.Trim() ?? string.Empty,
                    EmailAdd = bankContactViewModel.EmailAdd?.Trim() ?? string.Empty,
                    MessId = bankContactViewModel.MessId?.Trim() ?? string.Empty,
                    ContactMessType = bankContactViewModel.ContactMessType?.Trim() ?? string.Empty,
                    IsDefault = bankContactViewModel.IsDefault,
                    IsFinance = bankContactViewModel.IsFinance,
                    IsSales = bankContactViewModel.IsSales,
                    IsActive = bankContactViewModel.IsActive,
                    MobileNo = bankContactViewModel.MobileNo?.Trim() ?? string.Empty,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                var sqlResponse = await _BankContactService.SaveBankContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankContactEntity, headerViewModel.UserId);

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new Bank record");
            }
        }

        [HttpDelete, Route("DeleteBankContact/{BankId}/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<M_BankContact>> DeleteBankContact(Int32 BankId, Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);
                if (!userGroupRight.IsDelete)
                    return Unauthorized(GenerateMessage.AuthenticationFailed);

                var sqlResponse = await _BankContactService.DeleteBankContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, ContactId, headerViewModel.UserId);
                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}