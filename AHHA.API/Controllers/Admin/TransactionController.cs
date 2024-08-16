using AHHA.Application.IServices.Masters;
using AHHA.Application.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using AHHA.Core.Common;
using AHHA.Infra.Services.Admin;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authorization;
using System;
using AHHA.Core.Models.Admin;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;

        public TransactionController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TransactionController> logger, ITransactionService transactionService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        //        To get Submenu of the Module:
        //Get Call
        //http://118.189.194.191:8080/ahharestapiproject/ahha/getUserTransactions/{companyid}/{moduleid}/{userid}
        //http://118.189.194.191:8080/ahharestapiproject/ahha/getUserTransactions/1/1/1

        // [dbo].[Adm_GetUserTransactions_All]

        [HttpGet, Route("GetUsersTransactions")]
        public async Task<ActionResult> GetUsersTransactions([FromBody] ModuleViewModel Module)
        {
            try
            {
                //Convert Json file to object class
                //var people = JsonFileHelper.ReadFromJsonFile<Person>();
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var UsersTransactionsdata = await _transactionService.GetUsersTransactionsAsync(RegId,CompanyId, Module.ModuleId, UserId);

                    return Ok(UsersTransactionsdata);
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

        [HttpGet, Route("GetUsersTransactionsAll")]
        public async Task<ActionResult> GetUsersTransactionsAll()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId, CompanyId, UserId))
                {
                    var UsersTransactionsdata = await _transactionService.GetUsersTransactionsAllAsync(RegId,CompanyId, UserId);

                    return Ok(UsersTransactionsdata);
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

    }
}
