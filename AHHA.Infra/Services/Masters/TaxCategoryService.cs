using AHHA.Application.CommonServices;
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
    public sealed class TaxCategoryService : ITaxCategoryService
    {
        private readonly IRepository<M_TaxCategory> _repository;
        private ApplicationDbContext _context;

        public TaxCategoryService(IRepository<M_TaxCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            TaxCategoryViewModelCount taxCategoryViewModelCount = new TaxCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_TaxCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.TaxCategory}))");

                var result = await _repository.GetQueryAsync<TaxCategoryViewModel>(RegId, $"SELECT M_Cou.TaxCategoryId,M_Cou.TaxCategoryCode,M_Cou.TaxCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_TaxCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.TaxCategoryName LIKE '%{searchString}%' OR M_Cou.TaxCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.TaxCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.TaxCategory})) ORDER BY M_Cou.TaxCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                taxCategoryViewModelCount.responseCode = 200;
                taxCategoryViewModelCount.responseMessage = "success";
                taxCategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                taxCategoryViewModelCount.data = result == null ? null : result.ToList();

                return taxCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.TaxCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_TaxCategory> GetTaxCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 TaxCategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_TaxCategory>(RegId, $"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_TaxCategory WHERE TaxCategoryId={TaxCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.TaxCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddTaxCategoryAsync(string RegId, Int16 CompanyId, M_TaxCategory TaxCategory, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_TaxCategory WHERE CompanyId IN (SELECT DISTINCT TaxCategoryId FROM dbo.Fn_Adm_GetShareCompany ({TaxCategory.CompanyId},{(short)Modules.Master},{(short)Master.TaxCategory})) AND TaxCategoryCode='{TaxCategory.TaxCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_TaxCategory WHERE CompanyId IN (SELECT DISTINCT TaxCategoryId FROM dbo.Fn_Adm_GetShareCompany ({TaxCategory.CompanyId},{(short)Modules.Master},{(short)Master.TaxCategory})) AND TaxCategoryName='{TaxCategory.TaxCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "TaxCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "TaxCategory Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (TaxCategoryId + 1) FROM dbo.M_TaxCategory WHERE (TaxCategoryId + 1) NOT IN (SELECT TaxCategoryId FROM dbo.M_TaxCategory)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving TaxCategory

                        TaxCategory.TaxCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(TaxCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var TaxCategoryToSave = _context.SaveChanges();

                        #endregion Saving TaxCategory

                        #region Save AuditLog

                        if (TaxCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.TaxCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = TaxCategory.TaxCategoryId,
                                DocumentNo = TaxCategory.TaxCategoryCode,
                                TblName = "M_TaxCategory",
                                ModeId = (short)Mode.Create,
                                Remarks = "Tax Catgory Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "TaxCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.TaxCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = TaxCategory.TaxCategoryCode,
                        TblName = "M_TaxCategory",
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

        public async Task<SqlResponce> UpdateTaxCategoryAsync(string RegId, Int16 CompanyId, M_TaxCategory TaxCategory, Int32 UserId)
        {
            int IsActive = TaxCategory.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (TaxCategory.TaxCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_TaxCategory WHERE CompanyId IN (SELECT DISTINCT TaxCategoryId FROM dbo.Fn_Adm_GetShareCompany ({TaxCategory.CompanyId},{(short)Modules.Master},{(short)Master.TaxCategory})) AND TaxCategoryName='{TaxCategory.TaxCategoryName} AND TaxCategoryId <>{TaxCategory.TaxCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "TaxCategory Name Exist" };
                            }
                        }

                        #region Update TaxCategory

                        var entity = _context.Update(TaxCategory);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.TaxCategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update TaxCategory

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.TaxCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = TaxCategory.TaxCategoryId,
                                DocumentNo = TaxCategory.TaxCategoryCode,
                                TblName = "M_TaxCategory",
                                ModeId = (short)Mode.Update,
                                Remarks = "TaxCategory Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "TaxCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.TaxCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = TaxCategory.TaxCategoryId,
                        DocumentNo = TaxCategory.TaxCategoryCode,
                        TblName = "M_TaxCategory",
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

        public async Task<SqlResponce> DeleteTaxCategoryAsync(string RegId, Int16 CompanyId, M_TaxCategory TaxCategory, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (TaxCategory.TaxCategoryId > 0)
                    {
                        var TaxCategoryToRemove = _context.M_TaxCategory.Where(x => x.TaxCategoryId == TaxCategory.TaxCategoryId).ExecuteDelete();

                        if (TaxCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.TaxCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = TaxCategory.TaxCategoryId,
                                DocumentNo = TaxCategory.TaxCategoryCode,
                                TblName = "M_TaxCategory",
                                ModeId = (short)Mode.Delete,
                                Remarks = "TaxCategory Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "TaxCategoryId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.TaxCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_TaxCategory",
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