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
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _CategoryService;
        private readonly ILogger<CategoryController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public CategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CategoryController> logger, ICategoryService CategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CategoryService = CategoryService;
        }

        [HttpGet, Route("GetCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Category, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CategoryViewModelCount>("Category");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CategoryService.GetCategoryListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CategoryViewModelCount>("Category", cacheData, expirationTime);

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

        [HttpGet, Route("GetCategorybyid/{CategoryId}")]
        [Authorize]
        public async Task<ActionResult<CategoryViewModel>> GetCategoryById(Int16 CategoryId)
        {
            var CategoryViewModel = new CategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Category, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Category_{CategoryId}", out CategoryViewModel? cachedProduct))
                        {
                            CategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            CategoryViewModel = _mapper.Map<CategoryViewModel>(await _CategoryService.GetCategoryByIdAsync(RegId,CompanyId, CategoryId, UserId));

                            if (CategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the Category with an expiration time of 10 minutes
                                _memoryCache.Set($"Category_{CategoryId}", CategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CategoryViewModel);
                        //return Ok(CategoryViewModel);
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

        [HttpPost, Route("AddCategory")]
        [Authorize]
        public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryViewModel Category)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Category, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Category == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Category ID mismatch");

                            var CategoryEntity = new M_Category
                            {
                                CompanyId = Category.CompanyId,
                                CategoryCode = Category.CategoryCode,
                                CategoryId = Category.CategoryId,
                                CategoryName = Category.CategoryName,
                                CreateById = UserId,
                                IsActive = Category.IsActive,
                                Remarks = Category.Remarks
                            };

                            var createdCategory = await _CategoryService.AddCategoryAsync(RegId,CompanyId, CategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCategory);

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
                    "Error creating new Category record");
            }
        }

        [HttpPut, Route("UpdateCategory/{CategoryId}")]
        [Authorize]
        public async Task<ActionResult<CategoryViewModel>> UpdateCategory(Int16 CategoryId, [FromBody] CategoryViewModel Category)
        {
            var CategoryViewModel = new CategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Category, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CategoryId != Category.CategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Category ID mismatch");
                            //return BadRequest("M_Category ID mismatch");

                            // Attempt to retrieve the Category from the cache
                            if (_memoryCache.TryGetValue($"Category_{CategoryId}", out CategoryViewModel? cachedProduct))
                            {
                                CategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var CategoryToUpdate = await _CategoryService.GetCategoryByIdAsync(RegId,CompanyId, CategoryId, UserId);

                                if (CategoryToUpdate == null)
                                    return NotFound($"M_Category with Id = {CategoryId} not found");
                            }

                            var CategoryEntity = new M_Category
                            {
                                CategoryCode = Category.CategoryCode,
                                CategoryId = Category.CategoryId,
                                CategoryName = Category.CategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Category.IsActive,
                                Remarks = Category.Remarks
                            };

                            var sqlResponce = await _CategoryService.UpdateCategoryAsync(RegId,CompanyId, CategoryEntity, UserId);
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

        [HttpDelete, Route("Delete/{CategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_Category>> DeleteCategory(Int16 CategoryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Category, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CategoryToDelete = await _CategoryService.GetCategoryByIdAsync(RegId,CompanyId, CategoryId, UserId);

                            if (CategoryToDelete == null)
                                return NotFound($"M_Category with Id = {CategoryId} not found");

                            var sqlResponce = await _CategoryService.DeleteCategoryAsync(RegId,CompanyId, CategoryToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Category_{CategoryId}");
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

