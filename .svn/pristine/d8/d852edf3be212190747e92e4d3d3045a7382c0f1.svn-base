using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Application.IServices.Accounts.GL;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Account.GL;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Accounts.GL
{
    public sealed class GLOpeningBalanceService : IGLOpeningBalanceService
    {
        private readonly IRepository<GLBalances_GLCode> _repository;
        private ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public GLOpeningBalanceService(IRepository<GLBalances_GLCode> repository, ApplicationDbContext context, IAccountService accountService)
        {
            _repository = repository;
            _context = context;
            _accountService = accountService;
        }

        public async Task<IEnumerable<GLOpeningBalanceViewModel>> GetGLOpeningBalanceListAsync(string RegId, Int16 CompanyId, Int32 FinYear, Int16 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<GLOpeningBalanceViewModel>(RegId, $"SELECT Glopn.CompanyId,Glopn.CurrencyId,M_Curr.CurrencyCode,M_Curr.CurrencyName,Glopn.FinYear,Glopn.FinMonth,Glopn.GLId,M_CharAcc.GLCode,M_CharAcc.GLName,Glopn.IsDebit,Glopn.TotAmt,Glopn.TotLocalAmt FROM dbo.GLBalances_GLCode Glopn INNER JOIN dbo.M_Currency M_Curr ON M_Curr.CurrencyId = Glopn.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_CharAcc ON M_CharAcc.GLId = Glopn.GLId WHERE Glopn.FinYear\r\nr={FinYear}");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.OpeningBalance,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLBalances_GLCode",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //public async Task<SqlResponce> SaveGLOpeningBalanceAsync(string RegId, Int16 CompanyId, GLOpeningBalanceViewModel gLOpeningBalanceViewModel, Int16 UserId)
        //{
        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        bool IsEdit = false;
        //        try
        //        {
        //            if (gLOpeningBalanceViewModel.FinYear != 0)
        //            {
        //                IsEdit = true;
        //            }
        //            if (IsEdit)
        //            {
        //                var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier}))  AND SupplierCode='{gLOpeningBalanceViewModel.SupplierCode}' AND SupplierId <>{gLOpeningBalanceViewModel.SupplierId} UNION ALL SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) AND SupplierName='{gLOpeningBalanceViewModel.SupplierName}' AND SupplierId <>{gLOpeningBalanceViewModel.SupplierId}");
        //                if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
        //                    return new SqlResponce { Result = -1, Message = "Supplier Code or Name Exist" };
        //            }
        //            if (IsEdit)
        //            {
        //                var entityHead = _context.Update(gLOpeningBalanceViewModel);
        //                entityHead.Property(b => b.CreateById).IsModified = false;
        //                entityHead.Property(b => b.CompanyId).IsModified = false;
        //            }
        //            else
        //            {
        //                //Take the Next Id From SQL
        //                var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (SupplierId + 1) FROM dbo.M_Supplier WHERE (SupplierId + 1) NOT IN (SELECT SupplierId FROM dbo.M_Supplier)),1) AS NextId");
        //                if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
        //                {
        //                    gLOpeningBalanceViewModel.SupplierId = Convert.ToInt32(sqlMissingResponce.NextId);
        //                    gLOpeningBalanceViewModel.EditDate = null;
        //                    gLOpeningBalanceViewModel.EditById = null;
        //                    _context.Add(gLOpeningBalanceViewModel);
        //                }
        //                else
        //                    return new SqlResponce { Result = -1, Message = "SupplierId Should not be zero" };
        //            }

        //            var SupplierToSave = _context.SaveChanges();

        //            #region Save AuditLog

        //            if (SupplierToSave > 0)
        //            {
        //                //Saving Audit log
        //                var auditLog = new AdmAuditLog
        //                {
        //                    CompanyId = CompanyId,
        //                    ModuleId = (short)E_Modules.GL,
        //                    TransactionId = (short)E_GL.OpeningBalance,
        //                    DocumentId = gLOpeningBalanceViewModel.SupplierId,
        //                    DocumentNo = gLOpeningBalanceViewModel.SupplierCode,
        //                    TblName = "M_Supplier",
        //                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
        //                    Remarks = "Supplier Save Successfully",
        //                    CreateById = UserId,
        //                    CreateDate = DateTime.Now
        //                };

        //                _context.Add(auditLog);
        //                var auditLogSave = _context.SaveChanges();

        //                if (auditLogSave > 0)
        //                {
        //                    transaction.Commit();
        //                    return new SqlResponce { Result = 1, Message = "Save Successfully" };
        //                }
        //            }
        //            else
        //            {
        //                return new SqlResponce { Result = 1, Message = "Save Failed" };
        //            }

        //            #endregion Save AuditLog

        //            return new SqlResponce();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            _context.ChangeTracker.Clear();

        //            var errorLog = new AdmErrorLog
        //            {
        //                CompanyId = CompanyId,
        //                ModuleId = (short)E_Modules.GL,
        //                TransactionId = (short)E_GL.OpeningBalance,
        //                DocumentId = 0,
        //                DocumentNo = gLOpeningBalanceViewModel.SupplierCode,
        //                TblName = "M_Supplier",
        //                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
        //                Remarks = ex.Message + ex.InnerException,
        //                CreateById = UserId
        //            };
        //            _context.Add(errorLog);
        //            _context.SaveChanges();

        //            throw new Exception(ex.ToString());
        //        }
        //    }
        //}
    }
}