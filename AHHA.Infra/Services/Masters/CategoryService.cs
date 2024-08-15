using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
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

        public async Task<CategoryViewModelCount> GetCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CategoryViewModelCount CategoryViewModelCount = new CategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Category WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Category},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CategoryViewModel>($"SELECT M_Cou.CategoryId,M_Cou.CategoryCode,M_Cou.CategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Category M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CategoryName LIKE '%{searchString}%' OR M_Cou.CategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Category},{(short)Modules.Master})) ORDER BY M_Cou.CategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CategoryViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                CategoryViewModelCount.categoryViewModels = result == null ? null : result.ToList();

                return CategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Category,
                    TransactionId = (short)Modules.Master,
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
        public async Task<M_Category> GetCategoryByIdAsync(Int16 CompanyId, Int16 CategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Category>($"SELECT CategoryId,CategoryCode,CategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Category WHERE CategoryId={CategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Category,
                    TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> AddCategoryAsync(Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CategoryId FROM dbo.Fn_Adm_GetShareCompany ({Category.CompanyId},{(short)Master.Category},{(short)Modules.Master})) AND CategoryCode='{Category.CategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CategoryId FROM dbo.Fn_Adm_GetShareCompany ({Category.CompanyId},{(short)Master.Category},{(short)Modules.Master})) AND CategoryName='{Category.CategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Category Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Category Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CategoryId + 1) FROM dbo.M_Category WHERE (CategoryId + 1) NOT IN (SELECT CategoryId FROM dbo.M_Category)),1) AS MissId");

                        #region Saving Category

                        Category.CategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Category);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CategoryToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (CategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Category,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Category.CategoryId,
                                DocumentNo = Category.CategoryCode,
                                TblName = "M_Category",
                                ModeId = (short)Mode.Create,
                                Remarks = "Invoice Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            //await _auditLogServices.AddAuditLogAsync(auditLog);
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                sqlResponce = new SqlResponce { Id = 1, Message = "Save Successfully" };
                            }
                        }
                        #endregion

                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "CategoryId Should not be zero" };
                    }
                    return sqlResponce;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Category,
                        TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> UpdateCategoryAsync(Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            int IsActive = Category.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Category.CategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_Category WHERE CompanyId IN (SELECT DISTINCT CategoryId FROM dbo.Fn_Adm_GetShareCompany ({Category.CompanyId},{(short)Master.Category},{(short)Modules.Master})) AND CategoryName='{Category.CategoryName} AND CategoryId <>{Category.CategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Category Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Category

                            var entity = _context.Update(Category);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.CategoryCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Category,
                                    TransactionId = (short)Modules.Master,
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
                                    transaction.Commit();
                            }
                            sqlResponce = new SqlResponce { Id = 1, Message = "Update Successfully" };
                        }
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "CategoryId Should not be zero" };
                    }
                    return sqlResponce;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Category,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Category.CategoryId,
                        DocumentNo = Category.CategoryCode,
                        TblName = "M_Category",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    //await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
            }
        }
        public async Task<SqlResponce> DeleteCategoryAsync(Int16 CompanyId, M_Category Category, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
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
                            ModuleId = (short)Master.Category,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Category.CategoryId,
                            DocumentNo = Category.CategoryCode,
                            TblName = "M_Category",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Category Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "CategoryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Category,
                    TransactionId = (short)Modules.Master,
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
        public async Task<DataSet> GetTrainingByIdsAsync(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _repository.GetExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
