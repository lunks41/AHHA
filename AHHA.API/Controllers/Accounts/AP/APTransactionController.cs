﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.AP;
using AHHA.Core.Common;
using AHHA.Core.Models.Account;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.AP
{
    [Route("api/Account")]
    [ApiController]
    public class APTransactionController : BaseController
    {
        private readonly IAPTransactionService _APTransactionService;
        private readonly ILogger<APTransactionController> _logger;

        public APTransactionController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<APTransactionController> logger, IAPTransactionService APTransactionService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _APTransactionService = APTransactionService;
        }

        [HttpPost, Route("GetAPOutstandTransaction")]
        [Authorize]
        public async Task<ActionResult<GetTransactionViewModel>> GetAPOutstandTransactionList(GetTransactionViewModel getTransactionViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                var ARTransactionViewModel = await _APTransactionService.GetAPOutstandTransactionListAsync(headerViewModel.RegId, headerViewModel.CompanyId, getTransactionViewModel, headerViewModel.UserId);

                if (ARTransactionViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                return StatusCode(StatusCodes.Status202Accepted, ARTransactionViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }
    }
}