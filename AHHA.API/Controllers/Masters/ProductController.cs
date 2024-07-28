using AHHA.Application.Services.Masters.Products;
using AHHA.Core.Entities.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ProductController(ILogger<ProductController> logger, IProductService productService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _productService = productService;
            _memoryCache = memoryCache;
        }

        [HttpGet("getallproduct")]
        //[Route("getallproduct/{eid}/{WorksType}")]
        public async Task<ActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetProductListAsync();

                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
            
        }


        [HttpGet("getproductbyid/{id:int}")]
        public async Task<ActionResult<M_Product>> GetProductById(int id)
        {
            try
            {
                var result = await _productService.GetProductByIdAsync(id);

                if (result == null) return NotFound();

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost("addproduct")]
        public async Task<ActionResult<M_Product>> CreateProduct(M_Product product)
        {
            try
            {
                if (product == null)
                    return BadRequest();

                var createdProduct = await _productService.AddProductAsync(product);

                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new product record");
            }
        }

        [HttpPut("updateproduct/{id:int}")]
        public async Task<ActionResult<M_Product>> UpdateProduct(int id,[FromBody] M_Product product)
        {
            try
            {
                if (id != product.ProductId)
                    return BadRequest("M_Product ID mismatch");

                var productToUpdate = await _productService.GetProductByIdAsync(id);

                if (productToUpdate == null)
                    return NotFound($"M_Product with Id = {id} not found");

                await _productService.UpdateProductAsync(product);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete("deleteproduct/{id:int}")]
        public async Task<ActionResult<M_Product>> DeleteProduct(int id)
        {
            try
            {
                var productToDelete = await _productService.GetProductByIdAsync(id);

                if (productToDelete == null)
                {
                    return NotFound($"M_Product with Id = {id} not found");
                }

                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}
