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
    public class BankAddressController : BaseController
    {
        private readonly IBankAddressService _BankAddressService;
        private readonly ILogger<BankAddressController> _logger;

        public BankAddressController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BankAddressController> logger, IBankAddressService BankAddressService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BankAddressService = BankAddressService;
        }

        //Get the Bank Address List
        [HttpGet, Route("GetBankAddressbyBankId/{BankId}")]
        [Authorize]
        public async Task<ActionResult> GetBankAddressByBankId(Int16 BankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var cacheData = await _BankAddressService.GetBankAddressByBankIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId);

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

        //Bank Address one record by using addressId
        [HttpGet, Route("GetBankAddressbyid/{BankId}/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<BankAddressViewModel>> GetBankAddressById(Int32 BankId, Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var cacheData = await _BankAddressService.GetBankAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, AddressId, headerViewModel.UserId);

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

        [HttpPost, Route("SaveBankAddress")]
        [Authorize]
        public async Task<ActionResult<BankAddressViewModel>> SaveBankAddress(BankAddressViewModel bankAddressViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (bankAddressViewModel == null || bankAddressViewModel.BankId == 0)
                    return NotFound();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Bank, headerViewModel.UserId);

                if (userGroupRight == null || !userGroupRight.IsCreate)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var BankAddressEntity = new M_BankAddress
                {
                    BankId = bankAddressViewModel.BankId,
                    AddressId = bankAddressViewModel.AddressId,
                    Address1 = bankAddressViewModel.Address1?.Trim() ?? string.Empty,
                    Address2 = bankAddressViewModel.Address2?.Trim() ?? string.Empty,
                    Address3 = bankAddressViewModel.Address3?.Trim() ?? string.Empty,
                    Address4 = bankAddressViewModel.Address4?.Trim() ?? string.Empty,
                    PinCode = bankAddressViewModel.PinCode?.Trim() ?? string.Empty,
                    CountryId = bankAddressViewModel.CountryId,
                    PhoneNo = bankAddressViewModel.PhoneNo?.Trim() ?? string.Empty,
                    FaxNo = bankAddressViewModel.FaxNo?.Trim() ?? string.Empty,
                    EmailAdd = bankAddressViewModel.EmailAdd?.Trim() ?? string.Empty,
                    WebUrl = bankAddressViewModel.WebUrl?.Trim() ?? string.Empty,
                    IsDefaultAdd = bankAddressViewModel.IsDefaultAdd,
                    IsDeliveryAdd = bankAddressViewModel.IsDeliveryAdd,
                    IsFinAdd = bankAddressViewModel.IsFinAdd,
                    IsSalesAdd = bankAddressViewModel.IsSalesAdd,
                    IsActive = bankAddressViewModel.IsActive,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                var sqlResponse = await _BankAddressService.SaveBankAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankAddressEntity, headerViewModel.UserId);

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new BankAddress record");
            }
        }

        [HttpDelete, Route("DeleteBankAddress/{BankId}/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<M_BankAddress>> DeleteBankAddress(Int32 BankId, Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
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

                var sqlResponse = await _BankAddressService.DeleteBankAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, AddressId, headerViewModel.UserId);

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