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
    public sealed class CustomeGroupCreditLimtService : ICustomeGroupCreditLimtService
    {
        private readonly IRepository<M_CustomeGroupCreditLimt> _repository;
        private ApplicationDbContext _context;

        public CustomeGroupCreditLimtService(IRepository<M_CustomeGroupCreditLimt> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomeGroupCreditLimtViewModelCount> GetCustomeGroupCreditLimtListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomeGroupCreditLimtViewModelCount CustomeGroupCreditLimtViewModelCount = new CustomeGroupCreditLimtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_CustomeGroupCreditLimt WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.CustomerGroupCreditLimt},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CustomeGroupCreditLimtViewModel>($"SELECT M_Cou.GroupCreditLimitId,M_Cou.GroupCreditLimitCode,M_Cou.GroupCreditLimitName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomeGroupCreditLimt M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.GroupCreditLimitName LIKE '%{searchString}%' OR M_Cou.GroupCreditLimitCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.GroupCreditLimitId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.CustomerGroupCreditLimt},{(short)Modules.Master})) ORDER BY M_Cou.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CustomeGroupCreditLimtViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                CustomeGroupCreditLimtViewModelCount.customeGroupCreditLimtViewModels = result == null ? null : result.ToList();

                return CustomeGroupCreditLimtViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerGroupCreditLimt,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomeGroupCreditLimt",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_CustomeGroupCreditLimt> GetCustomeGroupCreditLimtByIdAsync(Int16 CompanyId, Int32 GroupCreditLimitId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomeGroupCreditLimt>($"SELECT GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomeGroupCreditLimt WHERE GroupCreditLimitId={GroupCreditLimitId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerGroupCreditLimt,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomeGroupCreditLimt",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCustomeGroupCreditLimtAsync(Int16 CompanyId, M_CustomeGroupCreditLimt CustomeGroupCreditLimt, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_CustomeGroupCreditLimt WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({CustomeGroupCreditLimt.CompanyId},{(short)Master.CustomerGroupCreditLimt},{(short)Modules.Master})) AND GroupCreditLimitCode='{CustomeGroupCreditLimt.GroupCreditLimitCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_CustomeGroupCreditLimt WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({CustomeGroupCreditLimt.CompanyId},{(short)Master.CustomerGroupCreditLimt},{(short)Modules.Master})) AND GroupCreditLimitName='{CustomeGroupCreditLimt.GroupCreditLimitName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "CustomeGroupCreditLimt Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "CustomeGroupCreditLimt Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_CustomeGroupCreditLimt WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_CustomeGroupCreditLimt)),1) AS MissId");

                        #region Saving CustomeGroupCreditLimt

                        CustomeGroupCreditLimt.GroupCreditLimitId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(CustomeGroupCreditLimt);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomeGroupCreditLimtToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (CustomeGroupCreditLimtToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.CustomerGroupCreditLimt,
                                TransactionId = (short)Modules.Master,
                                DocumentId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                DocumentNo = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                TblName = "M_CustomeGroupCreditLimt",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        ModuleId = (short)Master.CustomerGroupCreditLimt,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = CustomeGroupCreditLimt.GroupCreditLimitCode,
                        TblName = "M_CustomeGroupCreditLimt",
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
        public async Task<SqlResponce> UpdateCustomeGroupCreditLimtAsync(Int16 CompanyId, M_CustomeGroupCreditLimt CustomeGroupCreditLimt, Int32 UserId)
        {
            int IsActive = CustomeGroupCreditLimt.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomeGroupCreditLimt.GroupCreditLimitId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_CustomeGroupCreditLimt WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({CustomeGroupCreditLimt.CompanyId},{(short)Master.CustomerGroupCreditLimt},{(short)Modules.Master})) AND GroupCreditLimitName='{CustomeGroupCreditLimt.GroupCreditLimitName} AND GroupCreditLimitId <>{CustomeGroupCreditLimt.GroupCreditLimitId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "CustomeGroupCreditLimt Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update CustomeGroupCreditLimt

                            var entity = _context.Update(CustomeGroupCreditLimt);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.GroupCreditLimitCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.CustomerGroupCreditLimt,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                    DocumentNo = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                    TblName = "M_CustomeGroupCreditLimt",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "CustomeGroupCreditLimt Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        ModuleId = (short)Master.CustomerGroupCreditLimt,
                        TransactionId = (short)Modules.Master,
                        DocumentId = CustomeGroupCreditLimt.GroupCreditLimitId,
                        DocumentNo = CustomeGroupCreditLimt.GroupCreditLimitCode,
                        TblName = "M_CustomeGroupCreditLimt",
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
        public async Task<SqlResponce> DeleteCustomeGroupCreditLimtAsync(Int16 CompanyId, M_CustomeGroupCreditLimt CustomeGroupCreditLimt, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (CustomeGroupCreditLimt.GroupCreditLimitId > 0)
                {
                    var CustomeGroupCreditLimtToRemove = _context.M_CustomeGroupCreditLimt.Where(x => x.GroupCreditLimitId == CustomeGroupCreditLimt.GroupCreditLimitId).ExecuteDelete();

                    if (CustomeGroupCreditLimtToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.CustomerGroupCreditLimt,
                            TransactionId = (short)Modules.Master,
                            DocumentId = CustomeGroupCreditLimt.GroupCreditLimitId,
                            DocumentNo = CustomeGroupCreditLimt.GroupCreditLimitCode,
                            TblName = "M_CustomeGroupCreditLimt",
                            ModeId = (short)Mode.Delete,
                            Remarks = "CustomeGroupCreditLimt Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerGroupCreditLimt,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomeGroupCreditLimt",
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
