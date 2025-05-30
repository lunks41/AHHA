﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Helper;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AHHA.Infra.Services.Setting
{
    public sealed class DecimalSettingServices : IDecimalSettingService
    {
        private readonly IRepository<S_DecSettings> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DecimalSettingServices(IRepository<S_DecSettings> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<DecimalSettingViewModel> GetDecSettingAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DecimalSettingViewModel>(RegId, $"SELECT TOP (1) AmtDec,LocAmtDec,CtyAmtDec,PriceDec,QtyDec,ExhRateDec,DateFormat,LongDateFormat FROM dbo.S_DecSettings WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DecSetting}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DecSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDecSettingAsync(string RegId, Int16 CompanyId, S_DecSettings s_DecSettings, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.S_DecSettings WHERE CompanyId = {s_DecSettings.CompanyId}");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DecSettings);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DecSettings.EditById = null;
                        s_DecSettings.EditDate = null;
                        _context.Add(s_DecSettings);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DecSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DecSettings",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "FinSettings Save Successfully",
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
                        return new SqlResponse { Result = -1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DecSetting, 0, "", "S_DecSettings", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DecSetting, 0, "", "S_DecSettings", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}