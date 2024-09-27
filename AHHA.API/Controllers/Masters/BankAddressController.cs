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
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var BankAddressViewModel = await _BankAddressService.GetBankAddressByBankIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, headerViewModel.UserId);

                        if (BankAddressViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, BankAddressViewModel);
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

        //Bank Address one record by using addressId
        [HttpGet, Route("GetBankAddressbyid/{BankId}/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<BankAddressViewModel>> GetBankAddressById(Int16 BankId, Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var BankAddressViewModel = await _BankAddressService.GetBankAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, AddressId, headerViewModel.UserId);

                        if (BankAddressViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, BankAddressViewModel);
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

        [HttpPost, Route("SaveBankAddress")]
        [Authorize]
        public async Task<ActionResult<BankAddressViewModel>> SaveBankAddress(BankAddressViewModel BankAddressViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (BankAddressViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var BankAddressEntity = new M_BankAddress
                            {
                                BankId = BankAddressViewModel.BankId,
                                AddressId = BankAddressViewModel.AddressId,
                                Address1 = BankAddressViewModel.Address1,
                                Address2 = BankAddressViewModel.Address2 == null ? string.Empty : BankAddressViewModel.Address2.Trim(),
                                Address3 = BankAddressViewModel.Address3 == null ? string.Empty : BankAddressViewModel.Address3.Trim(),
                                Address4 = BankAddressViewModel.Address4 == null ? string.Empty : BankAddressViewModel.Address4.Trim(),
                                PinCode = BankAddressViewModel.PinCode == null ? string.Empty : BankAddressViewModel.PinCode.Trim(),
                                CountryId = BankAddressViewModel.CountryId,
                                PhoneNo = BankAddressViewModel.PhoneNo == null ? string.Empty : BankAddressViewModel.PhoneNo.Trim(),
                                FaxNo = BankAddressViewModel.FaxNo == null ? string.Empty : BankAddressViewModel.FaxNo.Trim(),
                                EmailAdd = BankAddressViewModel.EmailAdd == null ? string.Empty : BankAddressViewModel.EmailAdd.Trim(),
                                WebUrl = BankAddressViewModel.WebUrl == null ? string.Empty : BankAddressViewModel.WebUrl.Trim(),
                                IsDefaultAdd = BankAddressViewModel.IsDefaultAdd,
                                IsDeliveryAdd = BankAddressViewModel.IsDeliveryAdd,
                                IsFinAdd = BankAddressViewModel.IsFinAdd,
                                IsSalesAdd = BankAddressViewModel.IsSalesAdd,
                                IsActive = BankAddressViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _BankAddressService.SaveBankAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankAddressEntity, headerViewModel.UserId);

                            if (sqlResponce.Result > 0)
                            {
                                var BankModel = await _BankAddressService.GetBankAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankAddressViewModel.BankId, Convert.ToInt16(sqlResponce.Result), headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status202Accepted, BankModel);
                            }

                            return StatusCode(StatusCodes.Status204NoContent, sqlResponce);
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
                    "Error creating new BankAddress record");
            }
        }

        [HttpDelete, Route("DeleteBankAddress/{BankId}/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<M_BankAddress>> DeleteBankAddress(Int16 BankId, Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Bank, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var sqlResponce = await _BankAddressService.DeleteBankAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, BankId, AddressId, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"BankAddress_{AddressId}");
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