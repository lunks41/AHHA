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
    public sealed class TaxService : ITaxService
    {
        private readonly IRepository<M_Tax> _repository;
        private ApplicationDbContext _context;

        public TaxService(IRepository<M_Tax> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<TaxViewModelCount> GetTaxListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            TaxViewModelCount TaxViewModelCount = new TaxViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Tax WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Tax},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<TaxViewModel>($"SELECT M_Cou.TaxId,M_Cou.TaxCode,M_Cou.TaxName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Tax M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.TaxName LIKE '%{searchString}%' OR M_Cou.TaxCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.TaxId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Tax},{(short)Modules.Master})) ORDER BY M_Cou.TaxName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                TaxViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                TaxViewModelCount.taxViewModels = result == null ? null : result.ToList();

                return TaxViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Tax,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Tax> GetTaxByIdAsync(Int16 CompanyId, Int16 TaxId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Tax>($"SELECT TaxId,TaxCode,TaxName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Tax WHERE TaxId={TaxId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Tax,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddTaxAsync(Int16 CompanyId, M_Tax Tax, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Tax WHERE CompanyId IN (SELECT DISTINCT TaxId FROM dbo.Fn_Adm_GetShareCompany ({Tax.CompanyId},{(short)Master.Tax},{(short)Modules.Master})) AND TaxCode='{Tax.TaxCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Tax WHERE CompanyId IN (SELECT DISTINCT TaxId FROM dbo.Fn_Adm_GetShareCompany ({Tax.CompanyId},{(short)Master.Tax},{(short)Modules.Master})) AND TaxName='{Tax.TaxName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Tax Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Tax Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (TaxId + 1) FROM dbo.M_Tax WHERE (TaxId + 1) NOT IN (SELECT TaxId FROM dbo.M_Tax)),1) AS MissId");

                        #region Saving Tax

                        Tax.TaxId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Tax);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var TaxToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (TaxToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Tax,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Tax.TaxId,
                                DocumentNo = Tax.TaxCode,
                                TblName = "M_Tax",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "TaxId Should not be zero" };
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
                        ModuleId = (short)Master.Tax,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Tax.TaxCode,
                        TblName = "M_Tax",
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
        public async Task<SqlResponce> UpdateTaxAsync(Int16 CompanyId, M_Tax Tax, Int32 UserId)
        {
            int IsActive = Tax.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Tax.TaxId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_Tax WHERE CompanyId IN (SELECT DISTINCT TaxId FROM dbo.Fn_Adm_GetShareCompany ({Tax.CompanyId},{(short)Master.Tax},{(short)Modules.Master})) AND TaxName='{Tax.TaxName} AND TaxId <>{Tax.TaxId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Tax Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Tax

                            var entity = _context.Update(Tax);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.TaxCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Tax,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Tax.TaxId,
                                    DocumentNo = Tax.TaxCode,
                                    TblName = "M_Tax",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Tax Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "TaxId Should not be zero" };
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
                        ModuleId = (short)Master.Tax,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Tax.TaxId,
                        DocumentNo = Tax.TaxCode,
                        TblName = "M_Tax",
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
        public async Task<SqlResponce> DeleteTaxAsync(Int16 CompanyId, M_Tax Tax, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Tax.TaxId > 0)
                {
                    var TaxToRemove = _context.M_Tax.Where(x => x.TaxId == Tax.TaxId).ExecuteDelete();

                    if (TaxToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Tax,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Tax.TaxId,
                            DocumentNo = Tax.TaxCode,
                            TblName = "M_Tax",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Tax Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "TaxId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Tax,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<DataSet> GetTrainingByIdsAsync(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _repository.GetExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
