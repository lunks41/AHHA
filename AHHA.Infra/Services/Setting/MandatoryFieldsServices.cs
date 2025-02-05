﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AHHA.Infra.Services.Setting
{
    public sealed class MandatoryFieldsServices : IMandatoryFieldsServices
    {
        private readonly IRepository<S_MandatoryFields> _repository;
        private ApplicationDbContext _context;

        public MandatoryFieldsServices(IRepository<S_MandatoryFields> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<MandatoryFieldsViewModel>(RegId, $"SELECT CompanyId,ModuleId,TransactionId,M_ProductId,M_GLId,M_QTY,M_UomId,M_UnitPrice,M_TotAmt,M_Remarks,M_GstId,M_DeliveryDate,M_DepartmentId,M_EmployeeId,M_PortId,M_VesselId,M_BargeId,M_VoyageId,M_SupplyDate,M_ReferenceNo,M_SuppInvoiceNo,M_BankId,M_Remarks_Hd,M_Address1,M_Address2,M_Address3,M_Address4,M_PinCode,M_CountryId,M_PhoneNo,M_ContactName,M_MobileNo,M_EmailAdd FROM dbo.S_MandatoryFields S_Man LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_Man.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_Man.EditById WHERE ModuleId={ModuleId} AND CompanyId={CompanyId} AND TransactionId={TransactionId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.MandatoryFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_MandatoryFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<MandatoryFieldsViewModel>(RegId, $"SELECT ammod.ModuleId,ammod.ModuleName,amtrns.TransactionId,amtrns.TransactionName, S_man.M_ProductId M_ProductId,S_man.M_GLId M_GLId,S_man.M_QTY M_QTY,S_man.M_UomId M_UomId,S_man.M_UnitPrice M_UnitPrice,S_man.M_TotAmt M_TotAmt,S_man.M_Remarks M_Remarks,S_man.M_GstId,S_man.M_DeliveryDate M_DeliveryDate,S_man.M_DepartmentId M_DepartmentId,S_man.M_EmployeeId M_EmployeeId,S_man.M_PortId M_PortId,S_man.M_VesselId M_VesselId,S_man.M_BargeId M_BargeId,S_man.M_VoyageId M_VoyageId,S_man.M_SupplyDate M_SupplyDate,S_man.M_ReferenceNo M_ReferenceNo,S_man.M_SuppInvoiceNo M_SuppInvoiceNo,S_man.M_BankId M_BankId,S_man.M_Remarks_Hd M_Remarks_Hd,S_man.M_Address1 M_Address1,S_man.M_Address2 M_Address2,S_man.M_Address3 M_Address3,S_man.M_Address4 M_Address4,S_man.M_PinCode M_PinCode,S_man.M_CountryId M_CountryId,S_man.M_PhoneNo M_PhoneNo,S_man.M_ContactName M_ContactName,S_man.M_MobileNo M_MobileNo,S_man.M_EmailAdd M_EmailAdd " +
                    $"FROM  S_MandatoryFields S_man  " +
                    $"INNER JOIN AdmTransaction amtrns ON amtrns.ModuleId = S_man.ModuleId AND amtrns.TransactionId = S_man.TransactionId " +
                    $"INNER JOIN dbo.AdmModule ammod ON ammod.ModuleId = S_man.ModuleId " +
                    $"WHERE S_man.ModuleId = {ModuleId} Order By amtrns.TransactionId"
                    //$"SELECT ammod.ModuleId,ammod.ModuleName,amtrns.TransactionId,amtrns.TransactionName, ISNULL(S_man.M_ProductId,0) M_ProductId,ISNULL(S_man.M_GLId,0) M_GLId,ISNULL(S_man.M_QTY,0) M_QTY,ISNULL(S_man.M_UomId,0) M_UomId,ISNULL(S_man.M_UnitPrice,0) M_UnitPrice,ISNULL(S_man.M_TotAmt,0) M_TotAmt,ISNULL(S_man.M_Remarks,0) M_Remarks,ISNULL(S_man.M_GstId,0) M_GstId,ISNULL(S_man.M_DeliveryDate,0) M_DeliveryDate,ISNULL(S_man.M_DepartmentId,0) M_DepartmentId,ISNULL(S_man.M_EmployeeId,0) M_EmployeeId,ISNULL(S_man.M_PortId,0) M_PortId,ISNULL(S_man.M_VesselId,0) M_VesselId,ISNULL(S_man.M_BargeId,0) M_BargeId,ISNULL(S_man.M_VoyageId,0) M_VoyageId,ISNULL(S_man.M_SupplyDate,0) M_SupplyDate,ISNULL(S_man.M_ReferenceNo,0) M_ReferenceNo,ISNULL(S_man.M_SuppInvoiceNo,0) M_SuppInvoiceNo,ISNULL(S_man.M_BankId,0) M_BankId,ISNULL(S_man.M_Remarks_Hd,0) M_Remarks_Hd,ISNULL(S_man.M_Address1,0) M_Address1,ISNULL(S_man.M_Address2,0) M_Address2,ISNULL(S_man.M_Address3,0) M_Address3,ISNULL(S_man.M_Address4,0) M_Address4,ISNULL(S_man.M_PinCode,0) M_PinCode,ISNULL(S_man.M_CountryId,0) M_CountryId,ISNULL(S_man.M_PhoneNo,0) M_PhoneNo,ISNULL(S_man.M_ContactName,0) M_ContactName,ISNULL(S_man.M_MobileNo,0) M_MobileNo,ISNULL(S_man.M_EmailAdd,0) M_EmailAdd FROM  dbo.AdmTransaction amtrns INNER JOIN dbo.AdmModule ammod ON ammod.ModuleId = amtrns.ModuleId LEFT JOIN dbo.S_MandatoryFields S_man ON amtrns.TransactionId = S_man.TransactionId WHERE amtrns.ModuleId ={ModuleId} GROUP BY ISNULL(S_man.M_ProductId, 0), ISNULL(S_man.M_GLId, 0), ISNULL(S_man.M_QTY, 0),ISNULL(S_man.M_UomId, 0),ISNULL(S_man.M_UnitPrice, 0),ISNULL(S_man.M_TotAmt, 0),ISNULL(S_man.M_Remarks, 0),ISNULL(S_man.M_GstId, 0),ISNULL(S_man.M_DeliveryDate, 0),ISNULL(S_man.M_DepartmentId, 0),ISNULL(S_man.M_EmployeeId, 0),ISNULL(S_man.M_PortId, 0),ISNULL(S_man.M_VesselId, 0),ISNULL(S_man.M_BargeId, 0),ISNULL(S_man.M_VoyageId, 0), ISNULL(S_man.M_SupplyDate, 0),ISNULL(S_man.M_ReferenceNo, 0),ISNULL(S_man.M_SuppInvoiceNo, 0),ISNULL(S_man.M_BankId, 0),ISNULL(S_man.M_Remarks_Hd, 0),ISNULL(S_man.M_Address1, 0),ISNULL(S_man.M_Address2, 0),ISNULL(S_man.M_Address3, 0),ISNULL(S_man.M_Address4, 0),ISNULL(S_man.M_PinCode, 0),ISNULL(S_man.M_CountryId, 0),ISNULL(S_man.M_PhoneNo, 0),ISNULL(S_man.M_ContactName, 0),ISNULL(S_man.M_MobileNo, 0),ISNULL(S_man.M_EmailAdd, 0),ammod.ModuleId, ammod.ModuleName, amtrns.TransactionId, amtrns.TransactionName"
                    );

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.MandatoryFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_MandatoryFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveMandatoryFieldsAsync(string RegId, Int16 CompanyId, List<S_MandatoryFields> s_MandatoryFields, short UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Clear existing tracked entities
                    var trackedEntities = _context.ChangeTracker.Entries<S_MandatoryFields>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    //// Remove existing records
                    //var existingEntities = await _context.S_MandatoryFields
                    //    .Where(x => x.ModuleId == Convert.ToByte(s_MandatoryFields[0].ModuleId) && x.CompanyId == Convert.ToByte(CompanyId))
                    //    .ToListAsync();

                    //if (existingEntities.Any())
                    //{
                    //    _context.S_MandatoryFields.RemoveRange(existingEntities);
                    //    await _context.SaveChangesAsync();
                    //}

                    var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"DELETE FROM dbo.S_MandatoryFields WHERE CompanyId={CompanyId} AND ModuleId={s_MandatoryFields[0].ModuleId}");

                    // Add new records
                    _context.S_MandatoryFields.AddRange(s_MandatoryFields);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.MandatoryFields,
                            TblName = "S_MandatoryFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "MandatoryFields Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = 0, Message = "Save Failed" };
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.MandatoryFields,
                        TblName = "S_MandatoryFields",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty),
                        CreateById = UserId,
                        CreateDate = DateTime.Now
                    };

                    _context.AdmErrorLog.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> SaveMandatoryFieldsAsyncV1(string RegId, short CompanyId, List<S_MandatoryFields> s_MandatoryFields, short UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId,
                    //$"DELETE dbo.S_MandatoryFields WHERE ModuleId={s_MandatoryFields[0].ModuleId} AND CompanyId={CompanyId}");

                    _context.AddRange(s_MandatoryFields);

                    var mandatoryToSave = await _context.SaveChangesAsync();

                    #region Save AuditLog

                    if (mandatoryToSave > 0)
                    {
                        // Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.MandatoryFields,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_MandatoryFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "MandatoryFields Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 0, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse { Result = -1, Message = "Save Failed" };
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.MandatoryFields,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "S_MandatoryFields",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty),
                        CreateById = UserId,
                        CreateDate = DateTime.Now
                    };
                    _context.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw;
                }
            }
        }
    }
}