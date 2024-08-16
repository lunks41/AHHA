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
    public sealed class UomService : IUomService
    {
        private readonly IRepository<M_Uom> _repository;
        private ApplicationDbContext _context;

        public UomService(IRepository<M_Uom> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<UomViewModelCount> GetUomListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            UomViewModelCount UomViewModelCount = new UomViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Uom WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Uom},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<UomViewModel>(RegId,$"SELECT M_Cou.UomId,M_Cou.UomCode,M_Cou.UomName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Uom M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.UomName LIKE '%{searchString}%' OR M_Cou.UomCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.UomId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Uom},{(short)Modules.Master})) ORDER BY M_Cou.UomName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                UomViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                UomViewModelCount.uomViewModels = result == null ? null : result.ToList();

                return UomViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Uom,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Uom> GetUomByIdAsync(string RegId, Int16 CompanyId, Int16 UomId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Uom>(RegId,$"SELECT UomId,UomCode,UomName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Uom WHERE UomId={UomId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Uom,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddUomAsync(string RegId, Int16 CompanyId, M_Uom Uom, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Uom WHERE CompanyId IN (SELECT DISTINCT UomId FROM dbo.Fn_Adm_GetShareCompany ({Uom.CompanyId},{(short)Master.Uom},{(short)Modules.Master})) AND UomCode='{Uom.UomCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Uom WHERE CompanyId IN (SELECT DISTINCT UomId FROM dbo.Fn_Adm_GetShareCompany ({Uom.CompanyId},{(short)Master.Uom},{(short)Modules.Master})) AND UomName='{Uom.UomName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Uom Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Uom Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (UomId + 1) FROM dbo.M_Uom WHERE (UomId + 1) NOT IN (SELECT UomId FROM dbo.M_Uom)),1) AS MissId");

                        #region Saving Uom

                        Uom.UomId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Uom);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var UomToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (UomToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Uom,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Uom.UomId,
                                DocumentNo = Uom.UomCode,
                                TblName = "M_Uom",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "UomId Should not be zero" };
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
                        ModuleId = (short)Master.Uom,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Uom.UomCode,
                        TblName = "M_Uom",
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
        public async Task<SqlResponce> UpdateUomAsync(string RegId, Int16 CompanyId, M_Uom Uom, Int32 UserId)
        {
            int IsActive = Uom.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Uom.UomId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Uom WHERE CompanyId IN (SELECT DISTINCT UomId FROM dbo.Fn_Adm_GetShareCompany ({Uom.CompanyId},{(short)Master.Uom},{(short)Modules.Master})) AND UomName='{Uom.UomName} AND UomId <>{Uom.UomId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Uom Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Uom

                            var entity = _context.Update(Uom);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.UomCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Uom,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Uom.UomId,
                                    DocumentNo = Uom.UomCode,
                                    TblName = "M_Uom",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Uom Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "UomId Should not be zero" };
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
                        ModuleId = (short)Master.Uom,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Uom.UomId,
                        DocumentNo = Uom.UomCode,
                        TblName = "M_Uom",
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
        public async Task<SqlResponce> DeleteUomAsync(string RegId, Int16 CompanyId, M_Uom Uom, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Uom.UomId > 0)
                {
                    var UomToRemove = _context.M_Uom.Where(x => x.UomId == Uom.UomId).ExecuteDelete();

                    if (UomToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Uom,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Uom.UomId,
                            DocumentNo = Uom.UomCode,
                            TblName = "M_Uom",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Uom Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "UomId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Uom,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
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
