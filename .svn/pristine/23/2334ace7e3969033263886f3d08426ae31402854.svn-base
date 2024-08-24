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
    public sealed class SupplierService : ISupplierService
    {
        private readonly IRepository<M_Supplier> _repository;
        private ApplicationDbContext _context;

        public SupplierService(IRepository<M_Supplier> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SupplierViewModelCount> GetSupplierListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            SupplierViewModelCount SupplierViewModelCount = new SupplierViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId  WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Supplier},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<SupplierViewModel>(RegId, $"SSELECT M_Sup.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.IsCustomer,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.SupplierRegNo,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.CompanyId,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) ORDER BY M_Sup.SupplierName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY\"");

                SupplierViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                SupplierViewModelCount.data = result == null ? null : result.ToList();

                return SupplierViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Supplier> GetSupplierByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Supplier>(RegId, $"SELECT SupplierId,SupplierCode,SupplierName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Supplier WHERE SupplierId={SupplierId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierCode='{Supplier.SupplierCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierName='{Supplier.SupplierName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Supplier Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Supplier Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (SupplierId + 1) FROM dbo.M_Supplier WHERE (SupplierId + 1) NOT IN (SELECT SupplierId FROM dbo.M_Supplier)),1) AS MissId");

                        #region Saving Supplier

                        Supplier.SupplierId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Supplier);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var SupplierToSave = _context.SaveChanges();

                        #endregion Saving Supplier

                        #region Save AuditLog

                        if (SupplierToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Supplier,
                                DocumentId = Supplier.SupplierId,
                                DocumentNo = Supplier.SupplierCode,
                                TblName = "M_Supplier",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should not be zero" };
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
                        TransactionId = (short)Master.Supplier,
                        DocumentId = 0,
                        DocumentNo = Supplier.SupplierCode,
                        TblName = "M_Supplier",
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

        public async Task<SqlResponce> UpdateSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            int IsActive = Supplier.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Supplier.SupplierId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierName='{Supplier.SupplierName} AND SupplierId <>{Supplier.SupplierId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Supplier Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Supplier

                            var entity = _context.Update(Supplier);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.SupplierCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion Update Supplier

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Modules.Master,
                                    TransactionId = (short)Master.Supplier,
                                    DocumentId = Supplier.SupplierId,
                                    DocumentNo = Supplier.SupplierCode,
                                    TblName = "M_Supplier",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Supplier Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should not be zero" };
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
                        TransactionId = (short)Master.Supplier,
                        DocumentId = Supplier.SupplierId,
                        DocumentNo = Supplier.SupplierCode,
                        TblName = "M_Supplier",
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

        public async Task<SqlResponce> DeleteSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Supplier.SupplierId > 0)
                {
                    var SupplierToRemove = _context.M_Supplier.Where(x => x.SupplierId == Supplier.SupplierId).ExecuteDelete();

                    if (SupplierToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Modules.Master,
                            TransactionId = (short)Master.Supplier,
                            DocumentId = Supplier.SupplierId,
                            DocumentNo = Supplier.SupplierCode,
                            TblName = "M_Supplier",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Supplier Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should be zero" };
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
                    TransactionId = (short)Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
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