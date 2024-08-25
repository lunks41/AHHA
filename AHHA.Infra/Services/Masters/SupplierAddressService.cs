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
    public sealed class SupplierAddressService : ISupplierAddressService
    {
        private readonly IRepository<M_SupplierAddress> _repository;
        private ApplicationDbContext _context;

        public SupplierAddressService(IRepository<M_SupplierAddress> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SupplierAddressViewModelCount> GetSupplierAddressListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            SupplierAddressViewModelCount supplierAddressViewModelCount = new SupplierAddressViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_SupplierAddress WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Supplier}))");

                var result = await _repository.GetQueryAsync<SupplierAddressViewModel>(RegId, $"SELECT M_Cou.AddressId,M_Cou.SupplierAddressCode,M_Cou.SupplierAddressName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_SupplierAddress M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.SupplierAddressName LIKE '%{searchString}%' OR M_Cou.SupplierAddressCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.AddressId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Supplier})) ORDER BY M_Cou.SupplierAddressName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                supplierAddressViewModelCount.responseCode = 200;
                supplierAddressViewModelCount.responseMessage = "success";
                supplierAddressViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                supplierAddressViewModelCount.data = result == null ? null : result.ToList();

                return supplierAddressViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Supplier,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_SupplierAddress> GetSupplierAddressByIdAsync(string RegId, Int16 CompanyId, Int32 AddressId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_SupplierAddress>(RegId, $"SELECT AddressId,SupplierAddressCode,SupplierAddressName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_SupplierAddress WHERE AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Supplier,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress SupplierAddress, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_SupplierAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Supplier})) UNION ALL SELECT 2 AS IsExist FROM dbo.M_SupplierAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Supplier}))");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "SupplierAddress Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "SupplierAddress Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AddressId + 1) FROM dbo.M_SupplierAddress WHERE (AddressId + 1) NOT IN (SELECT AddressId FROM dbo.M_SupplierAddress)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving SupplierAddress

                        SupplierAddress.AddressId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(SupplierAddress);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var SupplierAddressToSave = _context.SaveChanges();

                        #endregion Saving SupplierAddress

                        #region Save AuditLog

                        if (SupplierAddressToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Supplier,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierAddress.SupplierId,
                                DocumentNo = "",
                                TblName = "M_SupplierAddress",
                                ModeId = (short)Mode.Create,
                                Remarks = "Supplier Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AddressId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Supplier,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = SupplierAddress.Address1,
                        TblName = "M_SupplierAddress",
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

        public async Task<SqlResponce> UpdateSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress SupplierAddress, Int32 UserId)
        {
            int IsActive = SupplierAddress.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SupplierAddress.AddressId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_SupplierAddress WHERE CompanyId IN (SELECT DISTINCT AddressId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Supplier})) AND SupplierAddressName='{SupplierAddress.Address1} AND AddressId <>{SupplierAddress.AddressId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "SupplierAddress Name Exist" };
                            }
                        }

                        #region Update SupplierAddress

                        var entity = _context.Update(SupplierAddress);

                        entity.Property(b => b.CreateById).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update SupplierAddress

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Supplier,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierAddress.AddressId,
                                DocumentNo = SupplierAddress.Address1,
                                TblName = "M_SupplierAddress",
                                ModeId = (short)Mode.Update,
                                Remarks = "SupplierAddress Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AddressId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Supplier,
                        TransactionId = (short)Modules.Master,
                        DocumentId = SupplierAddress.AddressId,
                        DocumentNo = SupplierAddress.Address1,
                        TblName = "M_SupplierAddress",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteSupplierAddressAsync(string RegId, Int16 CompanyId, M_SupplierAddress SupplierAddress, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SupplierAddress.AddressId > 0)
                    {
                        var SupplierAddressToRemove = _context.M_SupplierAddress.Where(x => x.AddressId == SupplierAddress.AddressId).ExecuteDelete();

                        if (SupplierAddressToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Supplier,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierAddress.AddressId,
                                DocumentNo = SupplierAddress.Address1,
                                TblName = "M_SupplierAddress",
                                ModeId = (short)Mode.Delete,
                                Remarks = "SupplierAddress Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AddressId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Supplier,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_SupplierAddress",
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
}