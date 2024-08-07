using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Entities.Masters;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Masters
{
    public class ProductService : IProductService
    {
        private readonly IRepository<M_Product> _repository;
        private readonly ApplicationDbContext _context;

        public ProductService(IRepository<M_Product> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<M_Product>> GetProductListAsync()
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        public async Task<M_Product> GetProductByIdAsync(int ProductId)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Product> AddProductAsync(M_Product product)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Product> UpdateProductAsync(M_Product product)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task DeleteProductAsync(int ProductId)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }


    //public class ProductService : IProductService
    //{
    //    private readonly DbContextClass _dbContext;

    //    public ProductService(DbContextClass dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }

    //    public async Task<List<M_Product>> GetProductListAsync()
    //    {
    //        return await _dbContext.M_Product
    //            .FromSqlRaw<M_Product>("SELECT * FROM dbo.M_Product")
    //            .ToListAsync();
    //    }

    //    public async Task<IEnumerable<M_Product>> GetProductByIdAsync(int ProductId)
    //    {
    //        var param = new SqlParameter("@ProductId", ProductId);

    //        var productDetails = await Task.Run(() => _dbContext.M_Product
    //                        .FromSqlRaw(@"exec GetPrductByID @ProductId", param).ToListAsync());

    //        return productDetails;
    //    }

    //    public async Task<int> AddProductAsync(M_Product product)
    //    {
    //        var parameter = new List<SqlParameter>();
    //        parameter.Add(new SqlParameter("@ProductName", product.ProductName));
    //        parameter.Add(new SqlParameter("@ProductDescription", product.ProductDescription));
    //        parameter.Add(new SqlParameter("@ProductPrice", product.ProductPrice));
    //        parameter.Add(new SqlParameter("@ProductStock", product.ProductStock));

    //        var result = await Task.Run(() => _dbContext.Database
    //       .ExecuteSqlRawAsync(@"exec AddNewProduct @ProductName, @ProductDescription, @ProductPrice, @ProductStock", parameter.ToArray()));

    //        return result;
    //    }

    //    public async Task<int> UpdateProductAsync(M_Product product)
    //    {
    //        var parameter = new List<SqlParameter>();
    //        parameter.Add(new SqlParameter("@ProductId", product.ProductId));
    //        parameter.Add(new SqlParameter("@ProductName", product.ProductName));
    //        parameter.Add(new SqlParameter("@ProductDescription", product.ProductDescription));
    //        parameter.Add(new SqlParameter("@ProductPrice", product.ProductPrice));
    //        parameter.Add(new SqlParameter("@ProductStock", product.ProductStock));

    //        var result = await Task.Run(() => _dbContext.Database
    //        .ExecuteSqlRawAsync(@"exec UpdateProduct @ProductId, @ProductName, @ProductDescription, @ProductPrice, @ProductStock", parameter.ToArray()));
    //        return result;
    //    }
    //    public async Task<int> DeleteProductAsync(int ProductId)
    //    {
    //        return await Task.Run(() => _dbContext.Database.ExecuteSqlInterpolatedAsync($"DeletePrductByID {ProductId}"));
    //    }
    //}
}
