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
    public sealed class VesselService : IVesselService
    {
        private readonly IRepository<M_Vessel> _repository;
        private ApplicationDbContext _context;

        public VesselService(IRepository<M_Vessel> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<VesselViewModelCount> GetVesselListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            VesselViewModelCount vesselViewModelCount = new VesselViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Vessel M_Vess WHERE (M_Vess.VesselName LIKE '%{searchString}%' OR M_Vess.VesselCode LIKE '%{searchString}%' OR M_Vess.VesselType LIKE '%{searchString}%') AND M_Vess.VesselId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Vessel}))");

                var result = await _repository.GetQueryAsync<VesselViewModel>(RegId, $"SELECT M_Vess.VesselId,M_Vess.CompanyId,M_Vess.VesselCode,M_Vess.VesselName,M_Vess.CallSign,M_Vess.IMOCode,M_Vess.GRT,M_Vess.LicenseNo,M_Vess.VesselType,M_Vess.Flag,M_Vess.Remarks,M_Vess.IsActive,M_Vess.CreateById,M_Vess.CreateDate,M_Vess.EditById,M_Vess.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Vessel M_Vess LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Vess.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Vess.EditById WHERE (M_Vess.VesselName LIKE '%{searchString}%' OR M_Vess.VesselCode LIKE '%{searchString}%' OR M_Vess.VesselType LIKE '%{searchString}%') AND M_Vess.VesselId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Vessel})) ORDER BY M_Vess.VesselName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                vesselViewModelCount.responseCode = 200;
                vesselViewModelCount.responseMessage = "success";
                vesselViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                vesselViewModelCount.data = result == null ? null : result.ToList();

                return vesselViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Vessel> GetVesselByIdAsync(string RegId, Int16 CompanyId, Int32 VesselId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Vessel>(RegId, $"SELECT VesselId,VesselCode,VesselName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Vessel WHERE VesselId={VesselId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddVesselAsync(string RegId, Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Modules.Master},{(short)Master.Vessel})) AND VesselCode='{Vessel.VesselCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Modules.Master},{(short)Master.Vessel})) AND VesselName='{Vessel.VesselName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Vessel Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Vessel Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (VesselId + 1) FROM dbo.M_Vessel WHERE (VesselId + 1) NOT IN (SELECT VesselId FROM dbo.M_Vessel)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Vessel

                        Vessel.VesselId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Vessel);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var VesselToSave = _context.SaveChanges();

                        #endregion Saving Vessel

                        #region Save AuditLog

                        if (VesselToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Vessel,
                                DocumentId = Vessel.VesselId,
                                DocumentNo = Vessel.VesselCode,
                                TblName = "M_Vessel",
                                ModeId = (short)Mode.Create,
                                Remarks = "Vessel Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VesselId Should not be zero" };
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
                        TransactionId = (short)Master.Vessel,
                        DocumentId = 0,
                        DocumentNo = Vessel.VesselCode,
                        TblName = "M_Vessel",
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

        public async Task<SqlResponce> UpdateVesselAsync(string RegId, Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Vessel.VesselId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Modules.Master},{(short)Master.Vessel})) AND VesselName='{Vessel.VesselName} AND VesselId <>{Vessel.VesselId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Vessel Name Exist" };
                            }
                        }

                        #region Update Vessel

                        var entity = _context.Update(Vessel);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.VesselCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Vessel

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Vessel,
                                DocumentId = Vessel.VesselId,
                                DocumentNo = Vessel.VesselCode,
                                TblName = "M_Vessel",
                                ModeId = (short)Mode.Update,
                                Remarks = "Vessel Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VesselId Should not be zero" };
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
                        TransactionId = (short)Master.Vessel,
                        DocumentId = Vessel.VesselId,
                        DocumentNo = Vessel.VesselCode,
                        TblName = "M_Vessel",
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

        public async Task<SqlResponce> DeleteVesselAsync(string RegId, Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Vessel.VesselId > 0)
                    {
                        var VesselToRemove = _context.M_Vessel.Where(x => x.VesselId == Vessel.VesselId).ExecuteDelete();

                        if (VesselToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Vessel,
                                DocumentId = Vessel.VesselId,
                                DocumentNo = Vessel.VesselCode,
                                TblName = "M_Vessel",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Vessel Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VesselId Should be zero" };
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
                        TransactionId = (short)Master.Vessel,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Vessel",
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