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
    public sealed class VoyageService : IVoyageService
    {
        private readonly IRepository<M_Voyage> _repository;
        private ApplicationDbContext _context;

        public VoyageService(IRepository<M_Voyage> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<VoyageViewModelCount> GetVoyageListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            VoyageViewModelCount voyageViewModelCount = new VoyageViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Voyage M_Voy INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = M_Voy.VesselId INNER JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = M_Voy.BargeId WHERE (M_Voy.ReferenceNo LIKE '%{searchString}%' OR M_Voy.VoyageNo LIKE '%{searchString}%' OR M_Voy.Remarks LIKE '%{searchString}%' OR M_Ves.VesselName LIKE '%{searchString}%' OR M_Ves.VesselCode LIKE '%{searchString}%' OR M_Bar.BargeName LIKE '%{searchString}%' OR M_Bar.BargeCode LIKE '%{searchString}%') AND M_Voy.VoyageId<>0 AND M_Voy.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Voyage}))");

                var result = await _repository.GetQueryAsync<VoyageViewModel>(RegId, $"SELECT M_Voy.VoyageId,M_Voy.VoyageNo,M_Voy.ReferenceNo,M_Ves.VesselCode,M_Ves.VesselName,M_Bar.BargeName,M_Bar.BargeCode,M_Voy.CompanyId,M_Voy.Remarks,M_Voy.IsActive,M_Voy.CreateById,M_Voy.CreateDate,M_Voy.EditById,M_Voy.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Voyage M_Voy INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = M_Voy.VesselId INNER JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = M_Voy.BargeId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Voy.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Voy.EditById WHERE (M_Voy.ReferenceNo LIKE '%{searchString}%' OR M_Voy.VoyageNo LIKE '%{searchString}%' OR M_Voy.Remarks LIKE '%{searchString}%' OR M_Ves.VesselName LIKE '%{searchString}%' OR M_Ves.VesselCode LIKE '%{searchString}%' OR M_Bar.BargeName LIKE '%{searchString}%' OR M_Bar.BargeCode LIKE '%{searchString}%') AND M_Voy.VoyageId<>0 AND M_Voy.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Voyage})) ORDER BY M_Voy.VoyageNo OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                voyageViewModelCount.responseCode = 200;
                voyageViewModelCount.responseMessage = "success";
                voyageViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                voyageViewModelCount.data = result == null ? null : result.ToList();

                return voyageViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Voyage> GetVoyageByIdAsync(string RegId, Int16 CompanyId, Int32 VoyageId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Voyage>(RegId, $"SELECT VoyageId,VoyageNo,VoyageName,CompanyId,ReferenceNo,VesselId,BargeId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Voyage WHERE VoyageId={VoyageId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Voyage}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddVoyageAsync(string RegId, Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Voyage})) AND VoyageNo='{Voyage.VoyageNo}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Voyage})) ");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Voyage Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Voyage Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (VoyageId + 1) FROM dbo.M_Voyage WHERE (VoyageId + 1) NOT IN (SELECT VoyageId FROM dbo.M_Voyage)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Voyage

                        Voyage.VoyageId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Voyage);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var VoyageToSave = _context.SaveChanges();

                        #endregion Saving Voyage

                        #region Save AuditLog

                        if (VoyageToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Voyage,
                                DocumentId = Voyage.VoyageId,
                                DocumentNo = Voyage.VoyageNo,
                                TblName = "M_Voyage",
                                ModeId = (short)Mode.Create,
                                Remarks = "Voyage Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VoyageId Should not be zero" };
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
                        TransactionId = (short)Master.Voyage,
                        DocumentId = 0,
                        DocumentNo = Voyage.VoyageNo,
                        TblName = "M_Voyage",
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

        public async Task<SqlResponce> UpdateVoyageAsync(string RegId, Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Voyage.VoyageId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Voyage})) AND VoyageNo='{Voyage.VoyageNo} AND VoyageId <>{Voyage.VoyageId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Voyage Name Exist" };
                            }
                        }

                        #region Update Voyage

                        var entity = _context.Update(Voyage);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.VoyageNo).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Voyage

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Voyage,
                                DocumentId = Voyage.VoyageId,
                                DocumentNo = Voyage.VoyageNo,
                                TblName = "M_Voyage",
                                ModeId = (short)Mode.Update,
                                Remarks = "Voyage Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VoyageId Should not be zero" };
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
                        TransactionId = (short)Master.Voyage,
                        DocumentId = Voyage.VoyageId,
                        DocumentNo = Voyage.VoyageNo,
                        TblName = "M_Voyage",
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

        public async Task<SqlResponce> DeleteVoyageAsync(string RegId, Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Voyage.VoyageId > 0)
                    {
                        var VoyageToRemove = _context.M_Voyage.Where(x => x.VoyageId == Voyage.VoyageId).ExecuteDelete();

                        if (VoyageToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Voyage,
                                DocumentId = Voyage.VoyageId,
                                DocumentNo = Voyage.VoyageNo,
                                TblName = "M_Voyage",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Voyage Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "VoyageId Should be zero" };
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
                        TransactionId = (short)Master.Voyage,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Voyage",
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