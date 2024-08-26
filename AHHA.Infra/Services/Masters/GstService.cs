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
    public sealed class GstService : IGstService
    {
        private readonly IRepository<M_Gst> _repository;
        private ApplicationDbContext _context;

        public GstService(IRepository<M_Gst> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GstViewModelCount> GetGstListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            GstViewModelCount gstViewModelCount = new GstViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.GstCategory}))");

                var result = await _repository.GetQueryAsync<GstViewModel>(RegId, $"SELECT M_Gt.GstId,M_Gt.GstCode,M_Gt.GstName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_gtc.GstCategoryCode,M_gtc.GstCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Gst})) ORDER BY M_Gt.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                gstViewModelCount.responseCode = 200;
                gstViewModelCount.responseMessage = "success";
                gstViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                gstViewModelCount.data = result == null ? null : result.ToList();

                return gstViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Gst> GetGstByIdAsync(string RegId, Int16 CompanyId, Int16 GstId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Gst>(RegId, $"SELECT GstId,GstCode,GstName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Gst WHERE GstId={GstId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddGstAsync(string RegId, Int16 CompanyId, M_Gst Gst, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Modules.Master},{(short)Master.Gst})) AND GstCode='{Gst.GstCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Modules.Master},{(short)Master.Gst})) AND GstName='{Gst.GstName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Gst Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Gst Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (GstId + 1) FROM dbo.M_Gst WHERE (GstId + 1) NOT IN (SELECT GstId FROM dbo.M_Gst)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Gst

                        Gst.GstId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Gst);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GstToSave = _context.SaveChanges();

                        #endregion Saving Gst

                        #region Save AuditLog

                        if (GstToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Gst,
                                DocumentId = Gst.GstId,
                                DocumentNo = Gst.GstCode,
                                TblName = "M_Gst",
                                ModeId = (short)Mode.Create,
                                Remarks = "Gst Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GstId Should not be zero" };
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
                        TransactionId = (short)Master.Gst,
                        DocumentId = 0,
                        DocumentNo = Gst.GstCode,
                        TblName = "M_Gst",
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

        public async Task<SqlResponce> UpdateGstAsync(string RegId, Int16 CompanyId, M_Gst Gst, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Gst.GstId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Modules.Master},{(short)Master.Gst})) AND GstName='{Gst.GstName} AND GstId <>{Gst.GstId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Gst Name Exist" };
                            }
                        }

                        #region Update Gst

                        var entity = _context.Update(Gst);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.GstCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Gst

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Gst,
                                DocumentId = Gst.GstId,
                                DocumentNo = Gst.GstCode,
                                TblName = "M_Gst",
                                ModeId = (short)Mode.Update,
                                Remarks = "Gst Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GstId Should not be zero" };
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
                        TransactionId = (short)Master.Gst,
                        DocumentId = Gst.GstId,
                        DocumentNo = Gst.GstCode,
                        TblName = "M_Gst",
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

        public async Task<SqlResponce> DeleteGstAsync(string RegId, Int16 CompanyId, M_Gst Gst, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Gst.GstId > 0)
                    {
                        var GstToRemove = _context.M_Gst.Where(x => x.GstId == Gst.GstId).ExecuteDelete();

                        if (GstToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Gst,
                                DocumentId = Gst.GstId,
                                DocumentNo = Gst.GstCode,
                                TblName = "M_Gst",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Gst Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GstId Should be zero" };
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
                        TransactionId = (short)Master.Gst,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Gst",
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