using AHHA.Core.Entities.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IProductService
    {
        public Task<IEnumerable<M_Product>> GetProductListAsync();
        public Task<M_Product> GetProductByIdAsync(int Id);
        public Task<M_Product> AddProductAsync(M_Product product);
        public Task<M_Product> UpdateProductAsync(M_Product product);
        public Task DeleteProductAsync(int Id);
    }
}
