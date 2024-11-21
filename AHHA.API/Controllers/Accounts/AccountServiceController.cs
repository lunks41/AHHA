using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Core.Common;
using AHHA.Core.Models.Account;
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
        public async Task<ActionResult> GetARAPPaymentHistory(HistoryViewModel historyViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                var ARAPPaymentHistory = await _AccountService.GetARAPPaymentHistoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, historyViewModel.ModuleId, historyViewModel.TransactionId, Convert.ToInt64(historyViewModel.DocumentId));

                if (ARAPPaymentHistory == null)
                    return NotFound(GenrateMessage.datanotfound);

                return StatusCode(StatusCodes.Status202Accepted, ARAPPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GLPostingHistory")]
        [Authorize]
        public async Task<ActionResult> GLPostingHistory(HistoryViewModel historyViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                var ARAPPaymentHistory = await _AccountService.GetGLPostingHistoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, historyViewModel.ModuleId, historyViewModel.TransactionId, Convert.ToInt64(historyViewModel.DocumentId));

                if (ARAPPaymentHistory == null)
                    return NotFound(GenrateMessage.datanotfound);

                return StatusCode(StatusCodes.Status202Accepted, ARAPPaymentHistory);
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