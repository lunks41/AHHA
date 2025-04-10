﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Masters
{
    public sealed class DesignationService : IDesignationService
    {
        private readonly IRepository<M_Designation> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DesignationService(IRepository<M_Designation> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<DesignationViewModelCount> GetDesignationListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            DesignationViewModelCount countViewModel = new DesignationViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Designation M_Des WHERE (M_Des.DesignationName LIKE '%{searchString}%' OR M_Des.DesignationCode LIKE '%{searchString}%' OR M_Des.Remarks LIKE '%{searchString}%') AND M_Des.DesignationId<>0 AND M_Des.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Designation}))");

                var result = await _repository.GetQueryAsync<DesignationViewModel>(RegId, $"SELECT M_Des.DesignationId,M_Des.CompanyId,M_Des.DesignationCode,M_Des.DesignationName,M_Des.CompanyId,M_Des.Remarks,M_Des.IsActive,M_Des.CreateById,M_Des.CreateDate,M_Des.EditById,M_Des.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Designation M_Des LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Des.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Des.EditById WHERE (M_Des.DesignationName LIKE '%{searchString}%' OR M_Des.DesignationCode LIKE '%{searchString}%' OR M_Des.Remarks LIKE '%{searchString}%') AND M_Des.DesignationId<>0 AND M_Des.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Designation})) ORDER BY M_Des.DesignationName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.Designation,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Designation> GetDesignationByIdAsync(string RegId, Int16 CompanyId, Int16 DesignationId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Designation>(RegId, $"SELECT DesignationId,DesignationCode,DesignationName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Designation WHERE DesignationId={DesignationId}  AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Designation})) ");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Designation,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDesignationAsync(string RegId, Int16 CompanyId, M_Designation m_Designation, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Designation.DesignationId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                        $"SELECT 1 AS IsExist FROM dbo.M_Designation WHERE DesignationId<>@DesignationId AND DesignationCode=@DesignationCode",
                        new { DesignationId = m_Designation.DesignationId, DesignationCode = m_Designation.DesignationCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Designation Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                        $"SELECT 1 AS IsExist FROM dbo.M_Designation WHERE DesignationId<>@DesignationId AND DesignationName=@DesignationName",
                        new { DesignationId = m_Designation.DesignationId, DesignationName = m_Designation.DesignationName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Designation Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                            $"SELECT 1 AS IsExist FROM dbo.M_Designation WHERE DesignationId=@DesignationId",
                            new { DesignationId = m_Designation.DesignationId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Designation);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Designation Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                            "SELECT ISNULL((SELECT TOP 1 (DesignationId + 1) FROM dbo.M_Designation WHERE (DesignationId + 1) NOT IN (SELECT DesignationId FROM dbo.M_Designation)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Designation.DesignationId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Designation);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Designation,
                            DocumentId = m_Designation.DesignationId,
                            DocumentNo = m_Designation.DesignationCode,
                            TblName = "M_Designation",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Designation Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Designation,
                        DocumentId = m_Designation.DesignationId,
                        DocumentNo = m_Designation.DesignationCode,
                        TblName = "AdmUser",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> DeleteDesignationAsync(string RegId, Int16 CompanyId, M_Designation Designation, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (Designation.DesignationId > 0)
                    {
                        var DesignationToRemove = _context.M_Designation.Where(x => x.DesignationId == Designation.DesignationId).ExecuteDelete();

                        if (DesignationToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Designation,
                                DocumentId = Designation.DesignationId,
                                DocumentNo = Designation.DesignationCode,
                                TblName = "M_Designation",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Designation Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "DesignationId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Designation,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Designation",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException?.Message,
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