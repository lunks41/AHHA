using AHHA.Application.IServices;
using AHHA.Core.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public readonly IMemoryCache _memoryCache;
        public readonly IMapper _mapper;
        public readonly IBaseService _baseServices;

        public BaseController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices)
        {
            _memoryCache = memoryCache;
            _mapper = mapper;
            _baseServices = baseServices;
        }

        [NonAction]
        public bool ValidateHeaders(Int16 CompanyId, Int32 UserId)
        {
            //RegID Check from json file
            //proper message for 
            bool IsValidate = false;

            if (CompanyId > 0 && UserId > 0)
            {
                IsValidate = true;
            }
            return IsValidate;
        }

        [NonAction]
        public UserGroupRightsViewModel ValidateScreen(Int16 CompanyId, Int16 ModuleId, Int32 TransactionId, Int32 UserId)
        {
            return _baseServices.ValidateScreen(CompanyId, ModuleId, TransactionId, UserId);
        }
    }
}
