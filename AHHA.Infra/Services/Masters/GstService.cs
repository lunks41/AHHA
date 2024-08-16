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
            GstViewModelCount GstViewModelCount = new GstViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Gst WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Gst},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<GstViewModel>(RegId,$"SELECT M_Cou.GstId,M_Cou.GstCode,M_Cou.GstName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.GstName LIKE '%{searchString}%' OR M_Cou.GstCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.GstId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Gst},{(short)Modules.Master})) ORDER BY M_Cou.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                GstViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                GstViewModelCount.gstViewModels = result == null ? null : result.ToList();

                return GstViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Gst,
                    TransactionId = (short)Modules.Master,
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
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Gst>(RegId,$"SELECT GstId,GstCode,GstName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Gst WHERE GstId={GstId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Gst,
                    TransactionId = (short)Modules.Master,
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
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT GstId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Master.Gst},{(short)Modules.Master})) AND GstCode='{Gst.GstCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT GstId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Master.Gst},{(short)Modules.Master})) AND GstName='{Gst.GstName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Gst Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Gst Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (GstId + 1) FROM dbo.M_Gst WHERE (GstId + 1) NOT IN (SELECT GstId FROM dbo.M_Gst)),1) AS MissId");

                        #region Saving Gst

                        Gst.GstId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Gst);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GstToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (GstToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Gst,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Gst.GstId,
                                DocumentNo = Gst.GstCode,
                                TblName = "M_Gst",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GstId Should not be zero" };
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
                        ModuleId = (short)Master.Gst,
                        TransactionId = (short)Modules.Master,
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
            int IsActive = Gst.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Gst.GstId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Gst WHERE CompanyId IN (SELECT DISTINCT GstId FROM dbo.Fn_Adm_GetShareCompany ({Gst.CompanyId},{(short)Master.Gst},{(short)Modules.Master})) AND GstName='{Gst.GstName} AND GstId <>{Gst.GstId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Gst Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Gst

                            var entity = _context.Update(Gst);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.GstCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Gst,
                                    TransactionId = (short)Modules.Master,
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
                                    transaction.Commit();
                            }
                            sqlResponce = new SqlResponce { Id = 1, Message = "Update Successfully" };
                        }
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "GstId Should not be zero" };
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
                        ModuleId = (short)Master.Gst,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Gst.GstId,
                        DocumentNo = Gst.GstCode,
                        TblName = "M_Gst",
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
        public async Task<SqlResponce> DeleteGstAsync(string RegId, Int16 CompanyId, M_Gst Gst, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
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
                            ModuleId = (short)Master.Gst,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Gst.GstId,
                            DocumentNo = Gst.GstCode,
                            TblName = "M_Gst",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Gst Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "GstId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Gst,
                    TransactionId = (short)Modules.Master,
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
