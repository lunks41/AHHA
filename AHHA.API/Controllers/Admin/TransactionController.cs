﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TransactionController> logger, ITransactionService transactionService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        //        To get Submenu of the Module:

        [HttpGet, Route("GetUsersTransactions")]
        public async Task<ActionResult> GetUsersTransactions([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UsersTransactionsdata = await _transactionService.GetUsersTransactionsAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.ModuleId, headerViewModel.UserId);

                    return Ok(UsersTransactionsdata);
                }
                else
                {
                    if (headerViewModel.UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetUsersTransactionsAll")]
        public async Task<ActionResult> GetUsersTransactionsAll([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UsersTransactionsdata = await _transactionService.GetUsersTransactionsAllAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return Ok(UsersTransactionsdata);
                }
                else
                {
                    if (headerViewModel.UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetMenu")]
        public async Task<ActionResult> GetMenu([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UsersTransactionsdata = await _transactionService.GetMenuListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return Ok(UsersTransactionsdata);
                }
                else
                {
                    if (headerViewModel.UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }
    }
}