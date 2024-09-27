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
    public sealed class PortService : IPortService
    {
        private readonly IRepository<M_Port> _repository;
        private ApplicationDbContext _context;

        public PortService(IRepository<M_Port> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortViewModelCount> GetPortListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            PortViewModelCount countViewModel = new PortViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Port M_Pro INNER JOIN dbo.M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId WHERE (M_Pro.PortName LIKE '%{searchString}%' OR M_Pro.PortCode LIKE '%{searchString}%' OR M_Pro.Remarks LIKE '%{searchString}%'OR M_Prg.PortRegionCode LIKE '%{searchString}%' OR M_Prg.PortRegionName LIKE '%{searchString}%') AND M_Pro.PortId<>0 AND M_Pro.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port}))");

                var result = await _repository.GetQueryAsync<PortViewModel>(RegId, $"SELECT M_Pro.PortId,M_Pro.CompanyId,M_Pro.PortRegionId,M_Prg.PortRegionCode,M_Prg.PortRegionName,M_Pro.PortCode,M_Pro.PortName,M_Pro.Remarks,M_Pro.IsActive,M_Pro.CreateById,M_Pro.CreateDate,M_Pro.EditById,M_Pro.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Port M_Pro INNER JOIN dbo.M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Pro.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Pro.EditById WHERE (M_Pro.PortName LIKE '%{searchString}%' OR M_Pro.PortCode LIKE '%{searchString}%' OR M_Pro.Remarks LIKE '%{searchString}%'OR M_Prg.PortRegionCode LIKE '%{searchString}%' OR M_Prg.PortRegionName LIKE '%{searchString}%') AND M_Pro.PortId<>0 AND M_Pro.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) ORDER BY M_Pro.PortName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<PortViewModel> GetPortByIdAsync(string RegId, Int16 CompanyId, Int16 PortId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<PortViewModel>(RegId, $"SELECT M_Pro.PortId,M_Pro.CompanyId,M_Pro.PortRegionId,M_Prg.PortRegionCode,M_Prg.PortRegionName,M_Pro.PortCode,M_Pro.PortName,M_Pro.Remarks,M_Pro.IsActive,M_Pro.CreateById,M_Pro.CreateDate,M_Pro.EditById,M_Pro.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Port M_Pro INNER JOIN dbo.M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Pro.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Pro.EditById WHERE PortId={PortId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddPortAsync(string RegId, Int16 CompanyId, M_Port m_Port, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) AND PortCode='{m_Port.PortCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) AND PortName='{m_Port.PortName}'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Port Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Port Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (PortId + 1) FROM dbo.M_Port WHERE (PortId + 1) NOT IN (SELECT PortId FROM dbo.M_Port)),1) AS NextId");
                    if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                    {
                        #region Saving Port

                        m_Port.PortId = Convert.ToInt16(sqlMissingResponce.NextId);

                        var entity = _context.Add(m_Port);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var PortToSave = _context.SaveChanges();

                        #endregion Saving Port

                        #region Save AuditLog

                        if (PortToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Port,
                                DocumentId = m_Port.PortId,
                                DocumentNo = m_Port.PortCode,
                                TblName = "M_Port",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Port Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "PortId Should not be zero" };
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
                        TransactionId = (short)E_Master.Port,
                        DocumentId = 0,
                        DocumentNo = m_Port.PortCode,
                        TblName = "M_Port",
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

        public async Task<SqlResponce> UpdatePortAsync(string RegId, Int16 CompanyId, M_Port Port, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Port.PortId > 0)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) AND PortName='{Port.PortName}' AND PortId <>{Port.PortId}");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Port Name Exist" };
                            }
                        }

                        #region Update Port

                        var entity = _context.Update(Port);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.PortCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Port

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Port,
                                DocumentId = Port.PortId,
                                DocumentNo = Port.PortCode,
                                TblName = "M_Port",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "Port Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "PortId Should not be zero" };
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
                        TransactionId = (short)E_Master.Port,
                        DocumentId = Port.PortId,
                        DocumentNo = Port.PortCode,
                        TblName = "M_Port",
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

        public async Task<SqlResponce> DeletePortAsync(string RegId, Int16 CompanyId, M_Port Port, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Port.PortId > 0)
                    {
                        var PortToRemove = _context.M_Port.Where(x => x.PortId == Port.PortId).ExecuteDelete();

                        if (PortToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Port,
                                DocumentId = Port.PortId,
                                DocumentNo = Port.PortCode,
                                TblName = "M_Port",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Port Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "PortId Should be zero" };
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
                        TransactionId = (short)E_Master.Port,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Port",
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