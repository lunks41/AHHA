using AHHA.Application.CommonServices;
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
    public sealed class CustomerAddressService : ICustomerAddressService
    {
        private readonly IRepository<M_CustomerAddress> _repository;
        private ApplicationDbContext _context;

        public CustomerAddressService(IRepository<M_CustomerAddress> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomerAddressViewModelCount> GetCustomerAddressListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomerAddressViewModelCount CustomerAddressViewModelCount = new CustomerAddressViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_CustomerAddress WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CustomerAddressViewModel>(RegId, $"SELECT M_Cou.AddressId,M_Cou.Address1,M_Cou.CustomerAddressName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerAddress M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CustomerAddressName LIKE '%{searchString}%' OR M_Cou.Address1 LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.AddressId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) ORDER BY M_Cou.CustomerAddressName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CustomerAddressViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                CustomerAddressViewModelCount.data = result == null ? null : result.ToList();

                return CustomerAddressViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_CustomerAddress> GetCustomerAddressByIdAsync(string RegId, Int16 CompanyId, Int32 AddressId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerAddress>(RegId, $"SELECT AddressId,Address1,CustomerAddressName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomerAddress WHERE AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master}))  UNION ALL SELECT 2 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master}))");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "CustomerAddress Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "CustomerAddress Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AddressId + 1) FROM dbo.M_CustomerAddress WHERE (AddressId + 1) NOT IN (SELECT AddressId FROM dbo.M_CustomerAddress)),1) AS MissId");

                        #region Saving CustomerAddress

                        CustomerAddress.AddressId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(CustomerAddress);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomerAddressToSave = _context.SaveChanges();

                        #endregion Saving CustomerAddress

                        #region Save AuditLog

                        if (CustomerAddressToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Customer,
                                DocumentId = CustomerAddress.AddressId,
                                DocumentNo = CustomerAddress.Address1,
                                TblName = "M_CustomerAddress",
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

                        #endregion Save AuditLog
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "AddressId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Customer,
                        DocumentId = 0,
                        DocumentNo = CustomerAddress.Address1,
                        TblName = "M_CustomerAddress",
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

        public async Task<SqlResponce> UpdateCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            int IsActive = CustomerAddress.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomerAddress.AddressId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND AddressId <>{CustomerAddress.AddressId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "CustomerAddress Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update CustomerAddress

                            var entity = _context.Update(CustomerAddress);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.Address1).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion Update CustomerAddress

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Modules.Master,
                                    TransactionId = (short)Master.Customer,
                                    DocumentId = CustomerAddress.AddressId,
                                    DocumentNo = CustomerAddress.Address1,
                                    TblName = "M_CustomerAddress",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "CustomerAddress Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "AddressId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Customer,
                        DocumentId = CustomerAddress.AddressId,
                        DocumentNo = CustomerAddress.Address1,
                        TblName = "M_CustomerAddress",
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

        public async Task<SqlResponce> DeleteCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (CustomerAddress.AddressId > 0)
                {
                    var CustomerAddressToRemove = _context.M_CustomerAddress.Where(x => x.AddressId == CustomerAddress.AddressId).ExecuteDelete();

                    if (CustomerAddressToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Modules.Master,
                            TransactionId = (short)Master.Customer,
                            DocumentId = CustomerAddress.AddressId,
                            DocumentNo = CustomerAddress.Address1,
                            TblName = "M_CustomerAddress",
                            ModeId = (short)Mode.Delete,
                            Remarks = "CustomerAddress Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "AddressId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
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