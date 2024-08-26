﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Services.Masters
{
    public sealed class ProductService : IProductService
    {
        private readonly IRepository<M_Product> _repository;
        private ApplicationDbContext _context;

        public ProductService(IRepository<M_Product> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ProductViewModelCount> GetProductListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            ProductViewModelCount productViewModelCount = new ProductViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Product M_Prod WHERE (M_Prod.ProductName LIKE '%{searchString}%' OR M_Prod.ProductCode LIKE '%{searchString}%' OR M_Prod.Remarks LIKE '%{searchString}%') AND M_Prod.ProductId<>0 AND M_Prod.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Product}))");

                var result = await _repository.GetQueryAsync<ProductViewModel>(RegId, $"SELECT M_Prod.ProductId,M_Prod.ProductCode,M_Prod.ProductName,M_Prod.CompanyId,M_Prod.Remarks,M_Prod.IsActive,M_Prod.CreateById,M_Prod.CreateDate,M_Prod.EditById,M_Prod.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Product M_Prod LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Prod.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Prod.EditById WHERE (M_Prod.ProductName LIKE '%{searchString}%' OR M_Prod.ProductCode LIKE '%{searchString}%' OR M_Prod.Remarks LIKE '%{searchString}%') AND M_Prod.ProductId<>0 AND M_Prod.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Product})) ORDER BY M_Prod.ProductName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                productViewModelCount.responseCode = 200;
                productViewModelCount.responseMessage = "success";
                productViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                productViewModelCount.data = result == null ? null : result.ToList();

                return productViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Product> GetProductByIdAsync(string RegId, Int16 CompanyId, Int16 ProductId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Product>(RegId, $"SELECT ProductId,ProductCode,ProductName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Product WHERE ProductId={ProductId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddProductAsync(string RegId, Int16 CompanyId, M_Product Product, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Product WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Product})) AND ProductCode='{Product.ProductCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Product WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Product})) AND ProductName='{Product.ProductName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Product Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Product Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (ProductId + 1) FROM dbo.M_Product WHERE (ProductId + 1) NOT IN (SELECT ProductId FROM dbo.M_Product)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Product

                        Product.ProductId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Product);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var ProductToSave = _context.SaveChanges();

                        #endregion Saving Product

                        #region Save AuditLog

                        if (ProductToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Product,
                                DocumentId = Product.ProductId,
                                DocumentNo = Product.ProductCode,
                                TblName = "M_Product",
                                ModeId = (short)Mode.Create,
                                Remarks = "Product Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "ProductId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Product,
                        DocumentId = 0,
                        DocumentNo = Product.ProductCode,
                        TblName = "M_Product",
                        ModeId = (short)Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> UpdateProductAsync(string RegId, Int16 CompanyId, M_Product Product, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Product.ProductId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Product WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Product})) AND ProductName='{Product.ProductName} AND ProductId <>{Product.ProductId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Product Name Exist" };
                            }
                        }

                        #region Update Product

                        var entity = _context.Update(Product);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.ProductCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Product

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Product,
                                DocumentId = Product.ProductId,
                                DocumentNo = Product.ProductCode,
                                TblName = "M_Product",
                                ModeId = (short)Mode.Update,
                                Remarks = "Product Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "ProductId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Product,
                        DocumentId = Product.ProductId,
                        DocumentNo = Product.ProductCode,
                        TblName = "M_Product",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteProductAsync(string RegId, Int16 CompanyId, M_Product Product, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Product.ProductId > 0)
                    {
                        var ProductToRemove = _context.M_Product.Where(x => x.ProductId == Product.ProductId).ExecuteDelete();

                        if (ProductToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Product,
                                DocumentId = Product.ProductId,
                                DocumentNo = Product.ProductCode,
                                TblName = "M_Product",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Product Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "ProductId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Product,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Product",
                        ModeId = (short)Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
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