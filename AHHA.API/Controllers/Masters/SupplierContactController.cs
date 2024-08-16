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
    public class SupplierContactController : BaseController
    {
        private readonly ISupplierContactService _SupplierContactService;
        private readonly ILogger<SupplierContactController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public SupplierContactController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierContactController> logger, ISupplierContactService SupplierContactService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierContactService = SupplierContactService;
        }

        [HttpGet, Route("GetSupplierContact")]
        [Authorize]
        public async Task<ActionResult> GetAllSupplierContact()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<SupplierContactViewModelCount>("SupplierContact");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _SupplierContactService.GetSupplierContactListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<SupplierContactViewModelCount>("SupplierContact", cacheData, expirationTime);

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

        [HttpGet, Route("GetSupplierContactbyid/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> GetSupplierContactById(Int16 ContactId)
        {
            var SupplierContactViewModel = new SupplierContactViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"SupplierContact_{ContactId}", out SupplierContactViewModel? cachedProduct))
                        {
                            SupplierContactViewModel = cachedProduct;
                        }
                        else
                        {
                            SupplierContactViewModel = _mapper.Map<SupplierContactViewModel>(await _SupplierContactService.GetSupplierContactByIdAsync(RegId,CompanyId, ContactId, UserId));

                            if (SupplierContactViewModel == null)
                                return NotFound();
                            else
                                // Cache the SupplierContact with an expiration time of 10 minutes
                                _memoryCache.Set($"SupplierContact_{ContactId}", SupplierContactViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SupplierContactViewModel);
                        //return Ok(SupplierContactViewModel);
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

        [HttpPost, Route("AddSupplierContact")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> CreateSupplierContact(SupplierContactViewModel SupplierContact)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (SupplierContact == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierContact ID mismatch");

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = SupplierContact.ContactId,
                                SupplierId = SupplierContact.SupplierId,
                                ContactName = SupplierContact.ContactName,
                                OtherName = SupplierContact.OtherName,
                                MobileNo = SupplierContact.MobileNo,
                                OffNo = SupplierContact.OffNo,
                                FaxNo = SupplierContact.FaxNo,
                                EmailAdd = SupplierContact.EmailAdd,
                                MessId = SupplierContact.MessId,
                                ContactMessType = SupplierContact.ContactMessType,
                                IsDefault = SupplierContact.IsDefault,
                                IsFinance = SupplierContact.IsFinance,
                                IsSales = SupplierContact.IsSales,
                                CreateById = UserId,
                                IsActive = SupplierContact.IsActive
                            };

                            var createdSupplierContact = await _SupplierContactService.AddSupplierContactAsync(RegId,CompanyId, SupplierContactEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplierContact);

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
                    "Error creating new SupplierContact record");
            }
        }

        [HttpPut, Route("UpdateSupplierContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> UpdateSupplierContact(Int16 ContactId, [FromBody] SupplierContactViewModel SupplierContact)
        {
            var SupplierContactViewModel = new SupplierContactViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (ContactId != SupplierContact.ContactId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierContact ID mismatch");
                            //return BadRequest("M_SupplierContact ID mismatch");

                            // Attempt to retrieve the SupplierContact from the cache
                            if (_memoryCache.TryGetValue($"SupplierContact_{ContactId}", out SupplierContactViewModel? cachedProduct))
                            {
                                SupplierContactViewModel = cachedProduct;
                            }
                            else
                            {
                                var SupplierContactToUpdate = await _SupplierContactService.GetSupplierContactByIdAsync(RegId,CompanyId, ContactId, UserId);

                                if (SupplierContactToUpdate == null)
                                    return NotFound($"M_SupplierContact with Id = {ContactId} not found");
                            }

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = SupplierContact.ContactId,
                                SupplierId = SupplierContact.SupplierId,
                                ContactName = SupplierContact.ContactName,
                                OtherName = SupplierContact.OtherName,
                                MobileNo = SupplierContact.MobileNo,
                                OffNo = SupplierContact.OffNo,
                                FaxNo = SupplierContact.FaxNo,
                                EmailAdd = SupplierContact.EmailAdd,
                                MessId = SupplierContact.MessId,
                                ContactMessType = SupplierContact.ContactMessType,
                                IsDefault = SupplierContact.IsDefault,
                                IsFinance = SupplierContact.IsFinance,
                                IsSales = SupplierContact.IsSales,
                                CreateById = UserId,
                                IsActive = SupplierContact.IsActive
                            };

                            var sqlResponce = await _SupplierContactService.UpdateSupplierContactAsync(RegId,CompanyId, SupplierContactEntity, UserId);
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

        [HttpDelete, Route("Delete/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<M_SupplierContact>> DeleteSupplierContact(Int16 ContactId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SupplierContactToDelete = await _SupplierContactService.GetSupplierContactByIdAsync(RegId,CompanyId, ContactId, UserId);

                            if (SupplierContactToDelete == null)
                                return NotFound($"M_SupplierContact with Id = {ContactId} not found");

                            var sqlResponce = await _SupplierContactService.DeleteSupplierContactAsync(RegId,CompanyId, SupplierContactToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"SupplierContact_{ContactId}");
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


