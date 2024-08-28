﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IRepository<M_Category> _repository;
        private ApplicationDbContext _context;

        public CategoryService(IRepository<M_Category> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CategoryViewModelCount> GetCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CategoryViewModelCount categoryViewModelCount = new CategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Category M_Cat WHERE (M_Cat.CategoryName LIKE '%{searchString}%' OR M_Cat.CategoryCode LIKE '%{searchString}%' OR M_Cat.Remarks LIKE '%{searchString}%') AND M_Cat.CategoryId<>0 AND M_Cat.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Category}))");

                var result = await _repository.GetQueryAsync<CategoryViewModel>(RegId, $"SELECT M_Cat.CategoryId,M_Cat.CompanyId,M_Cat.CategoryCode,M_Cat.CategoryName,M_Cat.Remarks,M_Cat.IsActive,M_Cat.CreateById,M_Cat.CreateDate,M_Cat.EditById,M_Cat.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Category M_Cat LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cat.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cat.EditById WHERE (M_Cat.CategoryName LIKE '%{searchString}%' OR M_Cat.CategoryCode LIKE '%{searchString}%' OR M_Cat.Remarks LIKE '%{searchString}%') AND M_Cat.CategoryId<>0 AND M_Cat.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Category})) ORDER BY M_Cat.CategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                categoryViewModelCount.responseCode = 200;
                categoryViewModelCount.responseMessage = "success";
                categoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                categoryViewModelCount.data = result == null ? null : result.ToList();

                return categoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Category> GetCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 CategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Category>(RegId, $"SELECT CategoryId,CompanyId,CategoryCode,CategoryName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Category WHERE CategoryId={CategoryId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Category}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCategoryAsync(string RegId, Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Category})) AND CategoryCode='{Category.CategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Category})) AND CategoryName='{Category.CategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Category Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Category Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (CategoryId + 1) FROM dbo.M_Category WHERE (CategoryId + 1) NOT IN (SELECT CategoryId FROM dbo.M_Category)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Category

                        Category.CategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Category);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CategoryToSave = _context.SaveChanges();

                        #endregion Saving Category

                        #region Save AuditLog

                        if (CategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Category,
                                DocumentId = Category.CategoryId,
                                DocumentNo = Category.CategoryCode,
                                TblName = "M_Category",
                                ModeId = (short)Mode.Create,
                                Remarks = "Category Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CategoryId Should not be zero" };
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
                        TransactionId = (short)Master.Category,
                        DocumentId = 0,
                        DocumentNo = Category.CategoryCode,
                        TblName = "M_Category",
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

        public async Task<SqlResponce> UpdateCategoryAsync(string RegId, Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Category.CategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Category})) AND CategoryName='{Category.CategoryName} AND CategoryId <>{Category.CategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Category Name Exist" };
                            }
                        }

                        #region Update Category

                        var entity = _context.Update(Category);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Category

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Category,
                                DocumentId = Category.CategoryId,
                                DocumentNo = Category.CategoryCode,
                                TblName = "M_Category",
                                ModeId = (short)Mode.Update,
                                Remarks = "Category Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CategoryId Should not be zero" };
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
                        TransactionId = (short)Master.Category,
                        DocumentId = Category.CategoryId,
                        DocumentNo = Category.CategoryCode,
                        TblName = "M_Category",
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

        public async Task<SqlResponce> DeleteCategoryAsync(string RegId, Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Category.CategoryId > 0)
                    {
                        var CategoryToRemove = _context.M_Category.Where(x => x.CategoryId == Category.CategoryId).ExecuteDelete();

                        if (CategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Category,
                                DocumentId = Category.CategoryId,
                                DocumentNo = Category.CategoryCode,
                                TblName = "M_Category",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Category Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CategoryId Should be zero" };
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
                        TransactionId = (short)Master.Category,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Category",
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