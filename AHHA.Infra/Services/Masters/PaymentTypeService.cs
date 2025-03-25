﻿using AHHA.Application.CommonServices;
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
    public sealed class PaymentTypeService : IPaymentTypeService
    {
        private readonly IRepository<M_PaymentType> _repository;
        private ApplicationDbContext _context;

        public PaymentTypeService(IRepository<M_PaymentType> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            PaymentTypeViewModelCount countViewModel = new PaymentTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_PaymentType M_Pay WHERE (M_Pay.PaymentTypeName LIKE '%{searchString}%' OR M_Pay.PaymentTypeCode LIKE '%{searchString}%' OR M_Pay.Remarks LIKE '%{searchString}%') AND M_Pay.PaymentTypeId<>0 AND M_Pay.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PaymentType}))");

                var result = await _repository.GetQueryAsync<PaymentTypeViewModel>(RegId, $"SELECT M_Pay.PaymentTypeId,M_Pay.CompanyId,M_Pay.PaymentTypeCode,M_Pay.PaymentTypeName,M_Pay.Remarks,M_Pay.IsActive,M_Pay.CreateById,M_Pay.CreateDate,M_Pay.EditById,M_Pay.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_PaymentType M_Pay LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Pay.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Pay.EditById WHERE (M_Pay.PaymentTypeName LIKE '%{searchString}%' OR M_Pay.PaymentTypeCode LIKE '%{searchString}%' OR M_Pay.Remarks LIKE '%{searchString}%') AND M_Pay.PaymentTypeId<>0 AND M_Pay.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PaymentType})) ORDER BY M_Pay.PaymentTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.PaymentType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_PaymentType> GetPaymentTypeByIdAsync(string RegId, Int16 CompanyId, Int16 PaymentTypeId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_PaymentType>(RegId, $"SELECT PaymentTypeId,PaymentTypeCode,PaymentTypeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_PaymentType WHERE PaymentTypeId={PaymentTypeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PaymentType}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PaymentType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SavePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType m_PaymentType, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_PaymentType.PaymentTypeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                        $"SELECT 1 AS IsExist FROM dbo.M_PaymentType WHERE PaymentTypeId<>@PaymentTypeId AND PaymentTypeCode=@PaymentTypeCode",
                        new { PaymentTypeId = m_PaymentType.PaymentTypeId, PaymentTypeCode = m_PaymentType.PaymentTypeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "PaymentType Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                        $"SELECT 1 AS IsExist FROM dbo.M_PaymentType WHERE PaymentTypeId<>@PaymentTypeId AND PaymentTypeName=@PaymentTypeName",
                        new { PaymentTypeId = m_PaymentType.PaymentTypeId, PaymentTypeName = m_PaymentType.PaymentTypeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "PaymentType Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                            $"SELECT 1 AS IsExist FROM dbo.M_PaymentType WHERE PaymentTypeId=@PaymentTypeId",
                            new { PaymentTypeId = m_PaymentType.PaymentTypeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_PaymentType);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "PaymentType Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId,
                            "SELECT ISNULL((SELECT TOP 1 (PaymentTypeId + 1) FROM dbo.M_PaymentType WHERE (PaymentTypeId + 1) NOT IN (SELECT PaymentTypeId FROM dbo.M_PaymentType)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_PaymentType.PaymentTypeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_PaymentType);
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
                            TransactionId = (short)E_Master.PaymentType,
                            DocumentId = m_PaymentType.PaymentTypeId,
                            DocumentNo = m_PaymentType.PaymentTypeCode,
                            TblName = "M_PaymentType",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "PaymentType Save Successfully",
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
                        TransactionId = (short)E_Master.PaymentType,
                        DocumentId = m_PaymentType.PaymentTypeId,
                        DocumentNo = m_PaymentType.PaymentTypeCode,
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

        public async Task<SqlResponse> DeletePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType PaymentType, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (PaymentType.PaymentTypeId > 0)
                    {
                        var PaymentTypeToRemove = _context.M_PaymentType.Where(x => x.PaymentTypeId == PaymentType.PaymentTypeId).ExecuteDelete();

                        if (PaymentTypeToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.PaymentType,
                                DocumentId = PaymentType.PaymentTypeId,
                                DocumentNo = PaymentType.PaymentTypeCode,
                                TblName = "M_PaymentType",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "PaymentType Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "PaymentTypeId Should be zero" };
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
                        TransactionId = (short)E_Master.PaymentType,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_PaymentType",
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