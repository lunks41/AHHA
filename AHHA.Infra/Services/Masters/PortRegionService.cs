﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Dapper.SqlMapper;

namespace AHHA.Infra.Services.Masters
{
    public sealed class PortRegionService : IPortRegionService
    {
        private readonly IRepository<M_PortRegion> _repository;
        private ApplicationDbContext _context;

        public PortRegionService(IRepository<M_PortRegion> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortRegionViewModelCount> GetPortRegionListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            PortRegionViewModelCount PortRegionViewModelCount = new PortRegionViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_PortRegion WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.PortRegion},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<PortRegionViewModel>($"SELECT M_PortRg.PortRegionId,M_PortRg.PortRegionCode,M_PortRg.PortRegionName,M_PortRg.CompanyId,M_PortRg.Remarks,M_PortRg.IsActive,M_PortRg.CreateById,M_PortRg.CreateDate,M_PortRg.EditById,M_PortRg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_PortRegion M_PortRg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_PortRg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_PortRg.EditById WHERE M_PortRg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) ORDER BY M_PortRg.PortRegionName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                PortRegionViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                PortRegionViewModelCount.portRegionViewModels = result == null ? null : result.ToList();

                return PortRegionViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_PortRegion> GetPortRegionByIdAsync(Int16 CompanyId, Int32 PortRegionId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_PortRegion>($"SELECT PortRegionId,PortRegionCode,PortRegionName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_PortRegion WHERE PortRegionId={PortRegionId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddPortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionCode='{PortRegion.PortRegionCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionName='{PortRegion.PortRegionName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "PortRegion Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "PortRegion Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (PortRegionId + 1) FROM dbo.M_PortRegion WHERE (PortRegionId + 1) NOT IN (SELECT PortRegionId FROM dbo.M_PortRegion)),1) AS MissId");

                        #region Saving PortRegion

                        PortRegion.PortRegionId = Convert.ToInt32(sqlMissingResponce.MissId);
                       
                        var entity = _context.Add(PortRegion);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var PortRegionToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (PortRegionToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.PortRegion,
                                TransactionId = (short)Modules.Master,
                                DocumentId = PortRegion.PortRegionId,
                                DocumentNo = PortRegion.PortRegionCode,
                                TblName = "M_PortRegion",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should not be zero" };
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
                        ModuleId = (short)Master.PortRegion,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = PortRegion.PortRegionCode,
                        TblName = "M_PortRegion",
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
        public async Task<SqlResponce> UpdatePortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            int IsActive = PortRegion.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (PortRegion.PortRegionId > 0)
                    {
                        //Check the Name exist or not
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionName='{PortRegion.PortRegionName} AND PortRegionId <>{PortRegion.PortRegionId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "PortRegion Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update PortRegion

                            var entity = _context.Update(PortRegion);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.PortRegionCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.PortRegion,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = PortRegion.PortRegionId,
                                    DocumentNo = PortRegion.PortRegionCode,
                                    TblName = "M_PortRegion",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "PortRegion Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should not be zero" };
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
                        ModuleId = (short)Master.PortRegion,
                        TransactionId = (short)Modules.Master,
                        DocumentId = PortRegion.PortRegionId,
                        DocumentNo = PortRegion.PortRegionCode,
                        TblName = "M_PortRegion",
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
        public async Task<SqlResponce> DeletePortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (PortRegion.PortRegionId > 0)
                {
                    var PortRegionToRemove = _context.M_PortRegion.Where(x => x.PortRegionId == PortRegion.PortRegionId).ExecuteDelete();

                    if (PortRegionToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.PortRegion,
                            TransactionId = (short)Modules.Master,
                            DocumentId = PortRegion.PortRegionId,
                            DocumentNo = PortRegion.PortRegionCode,
                            TblName = "M_PortRegion",
                            ModeId = (short)Mode.Delete,
                            Remarks = "PortRegion Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
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