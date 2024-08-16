using AHHA.Application.CommonServices;
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
    public sealed class PaymentTypeService : IPaymentTypeService
    {
        private readonly IRepository<M_PaymentType> _repository;
        private ApplicationDbContext _context;

        public PaymentTypeService(IRepository<M_PaymentType> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PaymentTypeViewModelCount> GetPaymentTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            PaymentTypeViewModelCount PaymentTypeViewModelCount = new PaymentTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_PaymentType WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Product},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<PaymentTypeViewModel>(RegId,$"SELECT M_Cou.PaymentTypeId,M_Cou.PaymentTypeCode,M_Cou.PaymentTypeName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_PaymentType M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.PaymentTypeName LIKE '%{searchString}%' OR M_Cou.PaymentTypeCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.PaymentTypeId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Product},{(short)Modules.Master})) ORDER BY M_Cou.PaymentTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                PaymentTypeViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                PaymentTypeViewModelCount.paymentTypeViewModels = result == null ? null : result.ToList();

                return PaymentTypeViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Product,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_PaymentType> GetPaymentTypeByIdAsync(string RegId, Int16 CompanyId, Int32 PaymentTypeId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_PaymentType>(RegId,$"SELECT PaymentTypeId,PaymentTypeCode,PaymentTypeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_PaymentType WHERE PaymentTypeId={PaymentTypeId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Product,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddPaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType PaymentType, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_PaymentType WHERE CompanyId IN (SELECT DISTINCT PaymentTypeId FROM dbo.Fn_Adm_GetShareCompany ({PaymentType.CompanyId},{(short)Master.Product},{(short)Modules.Master})) AND PaymentTypeCode='{PaymentType.PaymentTypeCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_PaymentType WHERE CompanyId IN (SELECT DISTINCT PaymentTypeId FROM dbo.Fn_Adm_GetShareCompany ({PaymentType.CompanyId},{(short)Master.Product},{(short)Modules.Master})) AND PaymentTypeName='{PaymentType.PaymentTypeName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "PaymentType Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "PaymentType Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (PaymentTypeId + 1) FROM dbo.M_PaymentType WHERE (PaymentTypeId + 1) NOT IN (SELECT PaymentTypeId FROM dbo.M_PaymentType)),1) AS MissId");

                        #region Saving PaymentType

                        PaymentType.PaymentTypeId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(PaymentType);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var PaymentTypeToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (PaymentTypeToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Product,
                                TransactionId = (short)Modules.Master,
                                DocumentId = PaymentType.PaymentTypeId,
                                DocumentNo = PaymentType.PaymentTypeCode,
                                TblName = "M_PaymentType",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PaymentTypeId Should not be zero" };
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
                        ModuleId = (short)Master.Product,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = PaymentType.PaymentTypeCode,
                        TblName = "M_PaymentType",
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
        public async Task<SqlResponce> UpdatePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType PaymentType, Int32 UserId)
        {
            int IsActive = PaymentType.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (PaymentType.PaymentTypeId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_PaymentType WHERE CompanyId IN (SELECT DISTINCT PaymentTypeId FROM dbo.Fn_Adm_GetShareCompany ({PaymentType.CompanyId},{(short)Master.Product},{(short)Modules.Master})) AND PaymentTypeName='{PaymentType.PaymentTypeName} AND PaymentTypeId <>{PaymentType.PaymentTypeId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "PaymentType Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update PaymentType

                            var entity = _context.Update(PaymentType);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.PaymentTypeCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Product,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = PaymentType.PaymentTypeId,
                                    DocumentNo = PaymentType.PaymentTypeCode,
                                    TblName = "M_PaymentType",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "PaymentType Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PaymentTypeId Should not be zero" };
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
                        ModuleId = (short)Master.Product,
                        TransactionId = (short)Modules.Master,
                        DocumentId = PaymentType.PaymentTypeId,
                        DocumentNo = PaymentType.PaymentTypeCode,
                        TblName = "M_PaymentType",
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
        public async Task<SqlResponce> DeletePaymentTypeAsync(string RegId, Int16 CompanyId, M_PaymentType PaymentType, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
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
                            ModuleId = (short)Master.Product,
                            TransactionId = (short)Modules.Master,
                            DocumentId = PaymentType.PaymentTypeId,
                            DocumentNo = PaymentType.PaymentTypeCode,
                            TblName = "M_PaymentType",
                            ModeId = (short)Mode.Delete,
                            Remarks = "PaymentType Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "PaymentTypeId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Product,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
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
