using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;

namespace AHHA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public readonly IMemoryCache _memoryCache;
        public readonly IMapper _mapper;
        public BaseController(IMemoryCache memoryCache, IMapper mapper)
        {
            _memoryCache = memoryCache;
            _mapper = mapper;
        }
        [HttpGet]
        public bool ValidateHeaders(Int16 CompanyId, Int32 UserId)
        {
            bool IsValidate = false;

            if (CompanyId > 0 && UserId > 0)
            {
                IsValidate = true;
            }
            return IsValidate;
        }
    }
}
