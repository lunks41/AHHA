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
    public sealed class SubCategoryService : ISubCategoryService
    {
        private readonly IRepository<M_SubCategory> _repository;
        private ApplicationDbContext _context;

        public SubCategoryService(IRepository<M_SubCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SubCategoryViewModelCount> GetSubCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            SubCategoryViewModelCount SubCategoryViewModelCount = new SubCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_SubCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.SubCategory},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<SubCategoryViewModel>(RegId, $"SELECT M_Cou.SubCategoryId,M_Cou.SubCategoryCode,M_Cou.SubCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_SubCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.SubCategoryName LIKE '%{searchString}%' OR M_Cou.SubCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.SubCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.SubCategory},{(short)Modules.Master})) ORDER BY M_Cou.SubCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                SubCategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                SubCategoryViewModelCount.data = result == null ? null : result.ToList();

                return SubCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.SubCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_SubCategory> GetSubCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 SubCategoryId, Int32 UserId)
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
                    ModuleId = (short)Master.SubCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT SubCategoryId FROM dbo.Fn_Adm_GetShareCompany ({SubCategory.CompanyId},{(short)Master.SubCategory},{(short)Modules.Master})) AND SubCategoryCode='{SubCategory.SubCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT SubCategoryId FROM dbo.Fn_Adm_GetShareCompany ({SubCategory.CompanyId},{(short)Master.SubCategory},{(short)Modules.Master})) AND SubCategoryName='{SubCategory.SubCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "SubCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "SubCategory Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (SubCategoryId + 1) FROM dbo.M_SubCategory WHERE (SubCategoryId + 1) NOT IN (SELECT SubCategoryId FROM dbo.M_SubCategory)),1) AS MissId");

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
                                ModuleId = (short)Master.SubCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SubCategory.SubCategoryId,
                                DocumentNo = SubCategory.SubCategoryCode,
                                TblName = "M_SubCategory",
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

                        #endregion Save AuditLog
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "SubCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.SubCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = SubCategory.SubCategoryCode,
                        TblName = "M_SubCategory",
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

        public async Task<SqlResponce> UpdateSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int32 UserId)
        {
            int IsActive = SubCategory.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SubCategory.SubCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_SubCategory WHERE CompanyId IN (SELECT DISTINCT SubCategoryId FROM dbo.Fn_Adm_GetShareCompany ({SubCategory.CompanyId},{(short)Master.SubCategory},{(short)Modules.Master})) AND SubCategoryName='{SubCategory.SubCategoryName} AND SubCategoryId <>{SubCategory.SubCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "SubCategory Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
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
                                    ModuleId = (short)Master.SubCategory,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = SubCategory.SubCategoryId,
                                    DocumentNo = SubCategory.SubCategoryCode,
                                    TblName = "M_SubCategory",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "SubCategory Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "SubCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.SubCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = SubCategory.SubCategoryId,
                        DocumentNo = SubCategory.SubCategoryCode,
                        TblName = "M_SubCategory",
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

        public async Task<SqlResponce> DeleteSubCategoryAsync(string RegId, Int16 CompanyId, M_SubCategory SubCategory, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
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
                            ModuleId = (short)Master.SubCategory,
                            TransactionId = (short)Modules.Master,
                            DocumentId = SubCategory.SubCategoryId,
                            DocumentNo = SubCategory.SubCategoryCode,
                            TblName = "M_SubCategory",
                            ModeId = (short)Mode.Delete,
                            Remarks = "SubCategory Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "SubCategoryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.SubCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
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