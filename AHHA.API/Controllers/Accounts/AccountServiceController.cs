﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts
{
    [Route("api/Account")]
    [ApiController]
    public class AccountServiceController : BaseController
    {
        private readonly IAccountService _AccountService;
        private readonly ILogger<AccountServiceController> _logger;

        public AccountServiceController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountServiceController> logger, IAccountService AccountService)
   : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountService = AccountService;
        }

        [HttpGet, Route("GetARAPPaymentHistory")]
        [Authorize]
        public async Task<ActionResult> GetARAPPaymentHistory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                var ARAPPaymentHistory = await _AccountService.GetARAPPaymentHistoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.ModuleId, headerViewModel.TransactionId, Convert.ToInt64(headerViewModel.DocumentId));

                if (ARAPPaymentHistory == null)
                    return NotFound(GenrateMessage.datanotfound);

                return StatusCode(StatusCodes.Status202Accepted, ARAPPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GLPostingHistory")]
        [Authorize]
        public async Task<ActionResult> GLPostingHistory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                var ARAPPaymentHistory = await _AccountService.GetGLPostingHistoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.ModuleId, headerViewModel.TransactionId, Convert.ToInt64(headerViewModel.DocumentId));

                if (ARAPPaymentHistory == null)
                    return NotFound(GenrateMessage.datanotfound);

                return StatusCode(StatusCodes.Status202Accepted, ARAPPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        //[HttpGet, Route("GetHistoryVersion")]
        //[Authorize]
        //public async Task<ActionResult> GetHistoryVersion([FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        var ARAPPaymentHistory = await _AccountService.GetHistoryVersionListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.ModuleId, headerViewModel.TransactionId, Convert.ToInt64(headerViewModel.DocumentId));

        //        if (ARAPPaymentHistory == null)
        //            return NotFound(GenrateMessage.datanotfound);

        //        return StatusCode(StatusCodes.Status202Accepted, ARAPPaymentHistory);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //         new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
        //    }
        //}
    }
}