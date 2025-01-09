﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Services.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ProductController> logger, IProductService productService) : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet, Route("GetProduct")]
        [Authorize]
        public async Task<ActionResult> GetProduct([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Product, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _productService.GetProductListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetProductbyid/{ProductId}")]
        [Authorize]
        public async Task<ActionResult<ProductViewModel>> GetProductById(Int16 ProductId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Product, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var productViewModel = _mapper.Map<ProductViewModel>(await _productService.GetProductByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductId, headerViewModel.UserId));

                        if (productViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, productViewModel);
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
                _logger.LogError(ex, ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("AddProduct")]
        [Authorize]
        public async Task<ActionResult<ProductViewModel>> CreateProduct(ProductViewModel productViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Product, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (productViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var ProductEntity = new M_Product
                            {
                                ProductId = productViewModel.ProductId,
                                CompanyId = headerViewModel.CompanyId,
                                ProductCode = productViewModel.ProductCode,
                                ProductName = productViewModel.ProductName,
                                Remarks = productViewModel.Remarks,
                                IsActive = productViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdProduct = await _productService.AddProductAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdProduct);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new Product record");
            }
        }

        [HttpPut, Route("UpdateProduct/{ProductId}")]
        [Authorize]
        public async Task<ActionResult<ProductViewModel>> UpdateProduct(Int16 ProductId, [FromBody] ProductViewModel productViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Product, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (ProductId != productViewModel.ProductId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Product ID mismatch");

                            var ProductToUpdate = await _productService.GetProductByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductId, headerViewModel.UserId);

                            if (ProductToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var ProductEntity = new M_Product
                            {
                                ProductId = productViewModel.ProductId,
                                CompanyId = headerViewModel.CompanyId,
                                ProductCode = productViewModel.ProductCode,
                                ProductName = productViewModel.ProductName,
                                Remarks = productViewModel.Remarks,
                                IsActive = productViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _productService.UpdateProductAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete, Route("DeleteProduct/{ProductId}")]
        [Authorize]
        public async Task<ActionResult<M_Product>> DeleteProduct(Int16 ProductId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Product, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var ProductToDelete = await _productService.GetProductByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductId, headerViewModel.UserId);

                            if (ProductToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponse = await _productService.DeleteProductAsync(headerViewModel.RegId, headerViewModel.CompanyId, ProductToDelete, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}

#region COMMENTS CODE

//[HttpGet("Getproduct")]
////[Route("Getproduct/{eid}/{WorksType}")]
//public async Task<ActionResult> GetProducts()
//{
//    try
//    {
//        var products = await _productService.GetProductListAsync();

//        return Ok(products);
//    }
//    catch (Exception)
//    {
//        return StatusCode(StatusCodes.Status500InternalServerError,
//         "Error retrieving data from the database");
//    }

//}

//[HttpGet("getproductbyid/{id:int}")]
//public async Task<ActionResult<M_Product>> GetProductById(int id)
//{
//    try
//    {
//        var result = await _productService.GetProductByIdAsync(id);

//        if (result == null) return NotFound(GenrateMessage.authenticationfailed);

//        return result;
//    }
//    catch (Exception)
//    {
//        return StatusCode(StatusCodes.Status500InternalServerError,
//            "Error retrieving data from the database");
//    }
//}

//[HttpPost("addproduct")]
//public async Task<ActionResult<M_Product>> CreateProduct(M_Product product)
//{
//    try
//    {
//        if (product == null)
//            return BadRequest();

//        var createdProduct = await _productService.AddProductAsync(product);

//        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
//    }
//    catch (Exception)
//    {
//        return StatusCode(StatusCodes.Status500InternalServerError,
//            "Error creating new product record");
//    }
//}

//[HttpPut("updateproduct/{id:int}")]
//public async Task<ActionResult<M_Product>> UpdateProduct(int id,[FromBody] M_Product product)
//{
//    try
//    {
//        if (id != product.ProductId)
//            return BadRequest("Product ID mismatch");

//        var productToUpdate = await _productService.GetProductByIdAsync(id);

//        if (productToUpdate == null)
//            return NotFound($"Product with Id = {id} not found");

//        await _productService.UpdateProductAsync(product);
//        return NoContent();
//    }
//    catch (Exception)
//    {
//        return StatusCode(StatusCodes.Status500InternalServerError,
//            "Error updating data");
//    }
//}

//[HttpDelete("deleteproduct/{id:int}")]
//public async Task<ActionResult<M_Product>> DeleteProduct(int id)
//{
//    try
//    {
//        var productToDelete = await _productService.GetProductByIdAsync(id);

//        if (productToDelete == null)
//        {
//            return NotFound($"Product with Id = {id} not found");
//        }

//        await _productService.DeleteProductAsync(id);
//        return NoContent();
//    }
//    catch (Exception)
//    {
//        return StatusCode(StatusCodes.Status500InternalServerError,
//            "Error deleting data");
//    }

#endregion COMMENTS CODE