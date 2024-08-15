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
    public sealed class CustomerContactService : ICustomerContactService
    {
        private readonly IRepository<M_CustomerContact> _repository;
        private ApplicationDbContext _context;

        public CustomerContactService(IRepository<M_CustomerContact> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomerContactViewModelCount> GetCustomerContactListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomerContactViewModelCount CustomerContactViewModelCount = new CustomerContactViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_CustomerContact WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CustomerContactViewModel>($"SELECT M_Cou.CustomerId,M_Cou.OtherName,M_Cou.ContactName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerContact M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.ContactName LIKE '%{searchString}%' OR M_Cou.OtherName LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CustomerId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) ORDER BY M_Cou.ContactName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CustomerContactViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                CustomerContactViewModelCount.customerContactViewModels = result == null ? null : result.ToList();

                return CustomerContactViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_CustomerContact> GetCustomerContactByIdAsync(Int16 CompanyId, Int32 CustomerId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerContact>($"SELECT CustomerId,OtherName,ContactName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomerContact WHERE CustomerId={CustomerId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCustomerContactAsync(Int16 CompanyId, M_CustomerContact CustomerContact, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_CustomerContact WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND OtherName='{CustomerContact.OtherName}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_CustomerContact WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND ContactName='{CustomerContact.ContactName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "CustomerContact Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "CustomerContact Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CustomerId + 1) FROM dbo.M_CustomerContact WHERE (CustomerId + 1) NOT IN (SELECT CustomerId FROM dbo.M_CustomerContact)),1) AS MissId");

                        #region Saving CustomerContact

                        CustomerContact.CustomerId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(CustomerContact);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomerContactToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (CustomerContactToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Customer,
                                TransactionId = (short)Modules.Master,
                                DocumentId = CustomerContact.CustomerId,
                                DocumentNo = CustomerContact.OtherName,
                                TblName = "M_CustomerContact",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should not be zero" };
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
                        ModuleId = (short)Master.Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = CustomerContact.OtherName,
                        TblName = "M_CustomerContact",
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
        public async Task<SqlResponce> UpdateCustomerContactAsync(Int16 CompanyId, M_CustomerContact CustomerContact, Int32 UserId)
        {
            int IsActive = CustomerContact.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomerContact.CustomerId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_CustomerContact WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND ContactName='{CustomerContact.ContactName} AND CustomerId <>{CustomerContact.CustomerId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "CustomerContact Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update CustomerContact

                            var entity = _context.Update(CustomerContact);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.OtherName).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Customer,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = CustomerContact.CustomerId,
                                    DocumentNo = CustomerContact.OtherName,
                                    TblName = "M_CustomerContact",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "CustomerContact Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should not be zero" };
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
                        ModuleId = (short)Master.Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = CustomerContact.CustomerId,
                        DocumentNo = CustomerContact.OtherName,
                        TblName = "M_CustomerContact",
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
        public async Task<SqlResponce> DeleteCustomerContactAsync(Int16 CompanyId, M_CustomerContact CustomerContact, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (CustomerContact.CustomerId > 0)
                {
                    var CustomerContactToRemove = _context.M_CustomerContact.Where(x => x.CustomerId == CustomerContact.CustomerId).ExecuteDelete();

                    if (CustomerContactToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Customer,
                            TransactionId = (short)Modules.Master,
                            DocumentId = CustomerContact.CustomerId,
                            DocumentNo = CustomerContact.OtherName,
                            TblName = "M_CustomerContact",
                            ModeId = (short)Mode.Delete,
                            Remarks = "CustomerContact Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
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
