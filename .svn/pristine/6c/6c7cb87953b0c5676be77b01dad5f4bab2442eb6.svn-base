using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Setting
{
    public sealed class VisibleFieldsServices : IVisibleFieldsServices
    {
        private readonly IRepository<S_VisibleFields> _repository;
        private ApplicationDbContext _context;

        public VisibleFieldsServices(IRepository<S_VisibleFields> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        //// add the number id
        //public async Task<VisibleFieldsViewModelCount> GetVisibleFieldsListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        //{
        //    VisibleFieldsViewModelCount countViewModel = new VisibleFieldsViewModelCount();
        //    try
        //    {
        //        var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.S_VisibleFields where CompanyId={CompanyId}");

        //        var result = await _repository.GetQueryAsync<VisibleFieldsViewModel>(RegId, $"CompanyId,ModuleId,TransactionId,M_ProductId,M_GLId,M_QTY,M_UomId,M_UnitPrice,M_TotAmt,M_Remarks,M_GstId,M_DeliveryDate,M_DepartmentId,M_EmployeeId,M_PortId,M_VesselId,M_BargeId,M_VoyageId,M_SupplyDate,M_ReferenceNo,M_SuppInvoiceNo,M_BankId,M_Remarks_Hd,M_Address1,M_Address2,M_Address3,M_Address4,M_PinCode,M_CountryId,M_PhoneNo,M_ContactName,M_MobileNo,M_EmailAdd FROM dbo.S_VisibleFields WHERE CompanyId={CompanyId}");

        //        countViewModel.responseCode = 200;
        //        countViewModel.responseMessage = "success";
        //        countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
        //        countViewModel.data = result == null ? null : result.ToList();

        //        return countViewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorLog = new AdmErrorLog
        //        {
        //            CompanyId = CompanyId,
        //            ModuleId = (short)E_Modules.Setting,
        //            TransactionId = (short)E_Setting.VisibleFields,
        //            DocumentId = 0,
        //            DocumentNo = "",
        //            TblName = "S_VisibleFields",
        //            ModeId = (short)E_Mode.View,
        //            Remarks = ex.Message + ex.InnerException,
        //            CreateById = UserId
        //        };

        //        _context.Add(errorLog);
        //        _context.SaveChanges();

        //        throw new Exception(ex.ToString());
        //    }
        //}

        public async Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<VisibleFieldsViewModel>(RegId, $"SELECT  S_Vis.CompanyId,S_Vis.ModuleId,S_Vis.TransactionId,S_Vis.M_ProductId,S_Vis.M_QTY,S_Vis.M_BillQTY,S_Vis.M_UomId,S_Vis.M_UnitPrice,S_Vis.M_GstId,S_Vis.M_DeliveryDate,S_Vis.M_DepartmentId,S_Vis.M_EmployeeId,S_Vis.M_PortId,S_Vis.M_VesselId,S_Vis.M_BargeId,S_Vis.M_VoyageId,S_Vis.M_SupplyDate,S_Vis.M_BankId,S_Vis.M_CtyCurr,S_Vis.CreateById,Usr.UserCode CreateBy ,S_Vis.CreateDate,S_Vis.EditById,Usr1.UserCode EditBy,S_Vis.EditDate FROM S_VisibleFields S_Vis Left Join AdmUser Usr on S_Vis.CreateById=Usr.UserId Left Join AdmUser Usr1 on S_Vis.EditById=Usr1.UserId WHERE ModuleId={ModuleId} AND CompanyId={CompanyId} AND TransactionId={TransactionId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.VisibleFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_VisibleFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveVisibleFieldsAsync(string RegId, Int16 CompanyId, S_VisibleFields s_VisibleFields, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (s_VisibleFields.CompanyId > 0 && s_VisibleFields.ModuleId > 0 && s_VisibleFields.TransactionId > 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.S_VisibleFields WHERE ModuleId={s_VisibleFields.ModuleId} AND TransactionId={s_VisibleFields.TransactionId} AND CompanyId={CompanyId}");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            _context.Update(s_VisibleFields);
                        }
                        else
                        {
                            _context.Add(s_VisibleFields);
                        }
                    }

                    var VisibleToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (VisibleToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.VisibleFields,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_VisibleFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "VisibleFields Save Successfully",
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

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.VisibleFields,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "S_VisibleFields",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }
    }
}