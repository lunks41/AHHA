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
    public sealed class SubCategoryService : ISubCategoryService
    {
        private readonly IRepository<M_SubCategory> _repository;
        private ApplicationDbContext _context;

        public SubCategoryService(IRepository<M_SubCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SubCategoryViewModelCount> GetSubCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            SubCategoryViewModelCount countViewModel = new SubCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_SubCategory M_Sub WHERE (M_Sub.SubCategoryName LIKE '%{searchString}%' OR M_Sub.SubCategoryCode LIKE '%{searchString}%' OR M_Sub.Remarks LIKE '%{searchString}%') AND M_Sub.SubCategoryId<>0 AND M_Sub.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory}))");

                var result = await _repository.GetQueryAsync<SubCategoryViewModel>(RegId, $"SELECT M_Sub.SubCategoryId,M_Sub.SubCategoryCode,M_Sub.SubCategoryName,M_Sub.CompanyId,M_Sub.Remarks,M_Sub.IsActive,M_Sub.CreateById,M_Sub.CreateDate,M_Sub.EditById,M_Sub.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_SubCategory M_Sub LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sub.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sub.EditById WHERE (M_Sub.SubCategoryName LIKE '%{searchString}%' OR M_Sub.SubCategoryCode LIKE '%{searchString}%' OR M_Sub.Remarks LIKE '%{searchString}%') AND M_Sub.SubCategoryId<>0 AND M_Sub.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) ORDER BY M_Sub.SubCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_SubCategory> GetSubCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 SubCategoryId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_SubCategory>(RegId, $"SELECT SubCategoryId,SubCategoryCode,SubCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_SubCategory WHERE SubCategoryId={SubCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) AND SubCategoryCode='{SubCategory.SubCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) AND SubCategoryName='{SubCategory.SubCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "SubCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "SubCategory Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (SubCategoryId + 1) FROM dbo.M_SubCategory WHERE (SubCategoryId + 1) NOT IN (SELECT SubCategoryId FROM dbo.M_SubCategory)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving SubCategory

                        SubCategory.SubCategoryId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(SubCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var SubCategoryToSave = _context.SaveChanges();

                        #endregion Saving SubCategory

                        #region Save AuditLog

                        if (SubCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.SubCategory,
                                DocumentId = SubCategory.SubCategoryId,
                                DocumentNo = SubCategory.SubCategoryCode,
                                TblName = "M_SubCategory",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Sub Category Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "SubCategoryId Should not be zero" };
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
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.SubCategory,
                        DocumentId = 0,
                        DocumentNo = SubCategory.SubCategoryCode,
                        TblName = "M_SubCategory",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> UpdateSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SubCategory.SubCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) AND SubCategoryName='{SubCategory.SubCategoryName} AND SubCategoryId <>{SubCategory.SubCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "SubCategory Name Exist" };
                            }
                        }

                        #region Update SubCategory

                        var entity = _context.Update(SubCategory);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.SubCategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update SubCategory

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.SubCategory,
                                DocumentId = SubCategory.SubCategoryId,
                                DocumentNo = SubCategory.SubCategoryCode,
                                TblName = "M_SubCategory",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "SubCategory Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "SubCategoryId Should not be zero" };
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
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.SubCategory,
                        DocumentId = SubCategory.SubCategoryId,
                        DocumentNo = SubCategory.SubCategoryCode,
                        TblName = "M_SubCategory",
                        ModeId = (short)E_Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SubCategory.SubCategoryId > 0)
                    {
                        var SubCategoryToRemove = _context.M_SubCategory.Where(x => x.SubCategoryId == SubCategory.SubCategoryId).ExecuteDelete();

                        if (SubCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.SubCategory,
                                DocumentId = SubCategory.SubCategoryId,
                                DocumentNo = SubCategory.SubCategoryCode,
                                TblName = "M_SubCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "SubCategory Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "SubCategoryId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Product,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Product",
                        ModeId = (short)E_Mode.Delete,
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