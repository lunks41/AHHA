﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IProductService
    {
        public Task<ProductViewModelCount> GetProductListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Product> GetProductByIdAsync(string RegId, Int16 CompanyId, Int16 ProductId, Int16 UserId);

        public Task<SqlResponse> SaveProductAsync(string RegId, Int16 CompanyId, M_Product M_Product, Int16 UserId);

        public Task<SqlResponse> DeleteProductAsync(string RegId, Int16 CompanyId, M_Product M_Product, Int16 UserId);
    }
}