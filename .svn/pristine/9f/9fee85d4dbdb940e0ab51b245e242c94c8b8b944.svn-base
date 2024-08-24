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
    public sealed class GstCategoryService : IGstCategoryService
    {
        private readonly IRepository<M_GstCategory> _repository;
        private ApplicationDbContext _context;

        public GstCategoryService(IRepository<M_GstCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GstCategoryViewModelCount> GetGstCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            GstCategoryViewModelCount GstCategoryViewModelCount = new GstCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_GstCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.GstCategory},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<GstCategoryViewModel>(RegId, $"SELECT M_Cou.GstCategoryId,M_Cou.GstCategoryCode,M_Cou.GstCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GstCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.GstCategoryName LIKE '%{searchString}%' OR M_Cou.GstCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.GstCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.GstCategory},{(short)Modules.Master})) ORDER BY M_Cou.GstCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                GstCategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                GstCategoryViewModelCount.data = result == null ? null : result.ToList();

                return GstCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GstCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_GstCategory> GetGstCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 GstCategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_GstCategory>(RegId, $"SELECT GstCategoryId,GstCategoryCode,GstCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GstCategory WHERE GstCategoryId={GstCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GstCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory GstCategory, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_GstCategory WHERE CompanyId IN (SELECT DISTINCT GstCategoryId FROM dbo.Fn_Adm_GetShareCompany ({GstCategory.CompanyId},{(short)Master.GstCategory},{(short)Modules.Master})) AND GstCategoryCode='{GstCategory.GstCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_GstCategory WHERE CompanyId IN (SELECT DISTINCT GstCategoryId FROM dbo.Fn_Adm_GetShareCompany ({GstCategory.CompanyId},{(short)Master.GstCategory},{(short)Modules.Master})) AND GstCategoryName='{GstCategory.GstCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "GstCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "GstCategory Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (GstCategoryId + 1) FROM dbo.M_GstCategory WHERE (GstCategoryId + 1) NOT IN (SELECT GstCategoryId FROM dbo.M_GstCategory)),1) AS MissId");

                        #region Saving GstCategory

                        GstCategory.GstCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(GstCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GstCategoryToSave = _context.SaveChanges();

                        #endregion Saving GstCategory

                        #region Save AuditLog

                        if (GstCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.GstCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = GstCategory.GstCategoryId,
                                DocumentNo = GstCategory.GstCategoryCode,
                                TblName = "M_GstCategory",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GstCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.GstCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = GstCategory.GstCategoryCode,
                        TblName = "M_GstCategory",
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

        public async Task<SqlResponce> UpdateGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory GstCategory, Int32 UserId)
        {
            int IsActive = GstCategory.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GstCategory.GstCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_GstCategory WHERE CompanyId IN (SELECT DISTINCT GstCategoryId FROM dbo.Fn_Adm_GetShareCompany ({GstCategory.CompanyId},{(short)Master.GstCategory},{(short)Modules.Master})) AND GstCategoryName='{GstCategory.GstCategoryName} AND GstCategoryId <>{GstCategory.GstCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "GstCategory Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update GstCategory

                            var entity = _context.Update(GstCategory);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.GstCategoryCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion Update GstCategory

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.GstCategory,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = GstCategory.GstCategoryId,
                                    DocumentNo = GstCategory.GstCategoryCode,
                                    TblName = "M_GstCategory",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "GstCategory Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GstCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.GstCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = GstCategory.GstCategoryId,
                        DocumentNo = GstCategory.GstCategoryCode,
                        TblName = "M_GstCategory",
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

        public async Task<SqlResponce> DeleteGstCategoryAsync(string RegId, Int16 CompanyId, M_GstCategory GstCategory, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (GstCategory.GstCategoryId > 0)
                {
                    var GstCategoryToRemove = _context.M_GstCategory.Where(x => x.GstCategoryId == GstCategory.GstCategoryId).ExecuteDelete();

                    if (GstCategoryToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.GstCategory,
                            TransactionId = (short)Modules.Master,
                            DocumentId = GstCategory.GstCategoryId,
                            DocumentNo = GstCategory.GstCategoryCode,
                            TblName = "M_GstCategory",
                            ModeId = (short)Mode.Delete,
                            Remarks = "GstCategory Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "GstCategoryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GstCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
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